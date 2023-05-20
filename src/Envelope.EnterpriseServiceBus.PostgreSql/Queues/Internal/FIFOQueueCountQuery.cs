using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Queues.Internal;

public class FIFOQueueCountQuery : ICompiledQuery<DbQueuedMessage, int>
{
	public Expression<Func<IMartenQueryable<DbQueuedMessage>, int>> QueryIs()
	{
		return q => q.Count();
	}
}
