using Envelope.EnterpriseServiceBus.Orchestrations;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class UnfinishedOrchestrationInstanceQuery : ICompiledListQuery<DbOrchestrationInstance, DbOrchestrationInstance>
{
	public Guid IdOrchestrationDefinition { get; set; }

	public Expression<Func<IMartenQueryable<DbOrchestrationInstance>, IEnumerable<DbOrchestrationInstance>>> QueryIs()
	{
		return q => q.Where(x => x.OrchestrationInstance.IdOrchestrationDefinition == IdOrchestrationDefinition
			&& (x.OrchestrationInstance.Status == OrchestrationStatus.Running
				|| x.OrchestrationInstance.Status == OrchestrationStatus.Executing));
	}
}
