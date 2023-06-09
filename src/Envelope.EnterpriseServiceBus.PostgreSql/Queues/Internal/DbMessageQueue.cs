﻿using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.PostgreSql.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Envelope.EnterpriseServiceBus.Queues;
using Envelope.ServiceBus.Messages;
using Envelope.Services;
using Envelope.Trace;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Queues.Internal;

internal class DbMessageQueue<TMessage> : IQueue<IQueuedMessage<TMessage>>
	where TMessage : class, IMessage
{
	private readonly bool _isFIFO;
	private bool _disposed;

	public int? MaxSize { get => null; set => _ = value; } //always null

	public DbMessageQueue(bool isFIFO)
	{
		_isFIFO = isFIFO;
	}

	public async Task<IResult<int>> GetCountAsync(ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<int>();

		if (transactionController == null)
			return result.WithArgumentNullException(traceInfo, nameof(transactionController));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();
		int count;

		if (_isFIFO)
			count = await martenSession.QueryAsync(new FIFOQueueCountQuery(), cancellationToken).ConfigureAwait(false);
		else
			count = await martenSession.QueryAsync(new DelayableQueueCountQuery(), cancellationToken).ConfigureAwait(false);

		return result.WithData(count).Build();
	}

	public Task<IResult> EnqueueAsync(List<IQueuedMessage<TMessage>> queuedMessages, ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder();

		if (queuedMessages == null)
			throw new ArgumentNullException(nameof(queuedMessages));

		if (transactionController == null)
			return Task.FromResult((IResult)result.WithArgumentNullException(traceInfo, nameof(transactionController)));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();

		var dbQueuedMessages = queuedMessages.Select(x =>
		{
			var msg = new QueuedMessageDto(x);
			var wrapper = new DbQueuedMessage
			{
				MessageId = msg.MessageId,
				QueuedMessage = msg,
			};
			return wrapper;
		});
		martenSession.Store(dbQueuedMessages);

		return Task.FromResult((IResult)result.Build());
	}

	public async Task<IResult<IQueuedMessage<TMessage>?>> TryPeekAsync(ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<IQueuedMessage<TMessage>?>();

		if (transactionController == null)
			return result.WithArgumentNullException(traceInfo, nameof(transactionController));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();

		DbQueuedMessage? existingDbQueuedMessage;

		if (_isFIFO)
			existingDbQueuedMessage = await martenSession.QueryAsync(new TryPeekFromFIFOQueueQuery(), cancellationToken).ConfigureAwait(false);
		else
			existingDbQueuedMessage = await martenSession.QueryAsync(new TryPeekFromDelayableQueueQuery(), cancellationToken).ConfigureAwait(false);

		if (existingDbQueuedMessage == null)
			return result.Build();

		var excahngeMessage = existingDbQueuedMessage.QueuedMessage?.ToQueuedMessage<TMessage>(traceInfo);
		return result.WithData(excahngeMessage).Build();
	}

	public async Task<IResult> TryRemoveAsync(IQueuedMessage<TMessage> queuedMessage, ITraceInfo traceInfo, ITransactionController transactionController, CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder();

		if (queuedMessage == null)
			return result.WithArgumentNullException(traceInfo, nameof(queuedMessage));

		if (transactionController == null)
			return result.WithArgumentNullException(traceInfo, nameof(transactionController));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();

		var existingDbQueuedMessage = await martenSession.LoadAsync<DbQueuedMessage>(queuedMessage.MessageId, cancellationToken).ConfigureAwait(false);
		if (existingDbQueuedMessage == null)
			return result.Build();

		martenSession.Delete(existingDbQueuedMessage);

		var dbQueuedArchivedMessage = new DbQueuedArchivedMessage
		{
			MessageId = existingDbQueuedMessage.MessageId,
			QueuedMessage = existingDbQueuedMessage.QueuedMessage,
		};

		martenSession.Store(dbQueuedArchivedMessage);

		return result.Build();
	}

	public async Task<IResult<QueueStatus>> UpdateAsync(IQueuedMessage<TMessage> queuedMessage, IMessageMetadataUpdate update, ITraceInfo traceInfo, ITransactionController localTransactionController, CancellationToken cancellationToken = default)
	{
		traceInfo = TraceInfo.Create(traceInfo);
		var result = new ResultBuilder<QueueStatus>();

		if (queuedMessage == null)
			return result.WithArgumentNullException(traceInfo, nameof(queuedMessage));

		if (update == null)
			return result.WithArgumentNullException(traceInfo, nameof(update));

		if (localTransactionController == null)
			return result.WithArgumentNullException(traceInfo, nameof(localTransactionController));

		var tc = localTransactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();

		var existingDbQueuedMessage = await martenSession.LoadAsync<DbQueuedMessage>(queuedMessage.MessageId, cancellationToken).ConfigureAwait(false);
		if (existingDbQueuedMessage == null)
			return result.WithInvalidOperationException(traceInfo, $"{nameof(existingDbQueuedMessage)} == null");

		var msg = new QueuedMessageDto(queuedMessage);
		msg.Update(update);
		existingDbQueuedMessage.QueuedMessage = msg;

		martenSession.Store(existingDbQueuedMessage);

		return 
			result
				.WithData((update.MessageStatus == MessageStatus.Suspended || update.MessageStatus == MessageStatus.Aborted)
					? QueueStatus.Suspended
					: QueueStatus.Running).Build();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
			return;

		_disposed = true;

		if (disposing)
		{
			//release managed resources
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
