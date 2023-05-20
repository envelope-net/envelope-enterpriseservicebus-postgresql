using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class OrchestrationInstanceByKeyQuery : ICompiledListQuery<DbOrchestrationInstance, DbOrchestrationInstance>
{
	public string? OrchestrationKey { get; set; }

	public Expression<Func<IMartenQueryable<DbOrchestrationInstance>, IEnumerable<DbOrchestrationInstance>>> QueryIs()
	{
		return q => q.Where(x => x.OrchestrationInstance.OrchestrationKey == OrchestrationKey);
	}
}
