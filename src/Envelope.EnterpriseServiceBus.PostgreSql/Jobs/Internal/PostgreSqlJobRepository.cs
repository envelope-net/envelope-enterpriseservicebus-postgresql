using Envelope.ServiceBus.Jobs;
using Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Envelope.Transactions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Jobs.Internal;

internal class PostgreSqlJobRepository : IJobRepository
{
	public async Task<TData?> LoadDataAsync<TData>(
		string jobName,
		ITransactionController transactionController,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(jobName))
			throw new ArgumentNullException(nameof(jobName));

		if (transactionController == null)
			throw new ArgumentNullException(nameof(transactionController));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();
		var dbJobData = await martenSession.QueryAsync(new JobDataByNameQuery { JobName = jobName }, cancellationToken).ConfigureAwait(false);

		if (dbJobData == null)
			return default;

		return (TData)dbJobData.Data;
	}

	public async Task SaveDataAsync<TData>(
		string jobName,
		TData? data,
		ITransactionController transactionController,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(jobName))
			throw new ArgumentNullException(nameof(jobName));

		if (transactionController == null)
			throw new ArgumentNullException(nameof(transactionController));

		var tc = transactionController.GetTransactionCache<PostgreSqlTransactionDocumentSessionCache>();
		var martenSession = tc.CreateOrGetSession();

		var dbJobData = await martenSession.QueryAsync(new JobDataByNameQuery { JobName = jobName }, cancellationToken).ConfigureAwait(false);
		if (dbJobData == null)
		{
			if (data != null)
			{
				dbJobData = new DbJobData().Initialize(jobName, data!);
				martenSession.Store(dbJobData);
			}
		}
		else
		{
			if (data == null)
			{
				martenSession.Delete(dbJobData.IdJobData);
			}
			else
			{
				dbJobData.Data = data;
				martenSession.Store(dbJobData);
			}
		}
	}
}
