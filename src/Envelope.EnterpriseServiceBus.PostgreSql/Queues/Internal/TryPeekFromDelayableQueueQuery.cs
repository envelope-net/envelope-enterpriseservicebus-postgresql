using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Queues.Internal;

public class TryPeekFromDelayableQueueQuery : ICompiledQuery<DbQueuedMessage, DbQueuedMessage?>
{
	public DateTime NowUtc { get; set; }

	public Expression<Func<IMartenQueryable<DbQueuedMessage>, DbQueuedMessage?>> QueryIs()
	{
		return q => q
			.OrderBy(x => x.QueuedMessage.PublishingTimeUtc)
			.FirstOrDefault(x =>
				x.QueuedMessage.MessageStatus != MessageStatus.Completed
				&& x.QueuedMessage.MessageStatus != MessageStatus.Discarded
				&& x.QueuedMessage.MessageStatus != MessageStatus.Aborted
				&& x.QueuedMessage.MessageStatus != MessageStatus.Suspended
				&& (!x.QueuedMessage.DelayedToUtc.HasValue
					|| x.QueuedMessage.DelayedToUtc < NowUtc));
	}
}
