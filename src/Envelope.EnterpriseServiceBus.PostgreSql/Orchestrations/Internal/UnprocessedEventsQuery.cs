using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Orchestrations.Internal;

public class UnprocessedEventsQuery : ICompiledListQuery<DbOrchestrationEvent, DbOrchestrationEvent>
{
	public Expression<Func<IMartenQueryable<DbOrchestrationEvent>, IEnumerable<DbOrchestrationEvent>>> QueryIs()
	{
		return q => q.Where(x => !x.OrchestrationEventMessage.ProcessedUtc.HasValue);
	}
}
