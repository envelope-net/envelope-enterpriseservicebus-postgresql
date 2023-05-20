using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class JobDataByNameQuery : ICompiledQuery<DbJobData, DbJobData?>
{
	public string? JobName { get; set; }

	public Expression<Func<IMartenQueryable<DbJobData>, DbJobData?>> QueryIs()
	{
		return q => q.FirstOrDefault(x => x.JobName == JobName);
	}
}
