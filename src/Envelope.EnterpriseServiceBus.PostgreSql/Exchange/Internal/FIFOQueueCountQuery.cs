using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class FIFOQueueCountQuery : ICompiledQuery<DbExchangeMessage, int>
{
	public Expression<Func<IMartenQueryable<DbExchangeMessage>, int>> QueryIs()
	{
		return q => q.Count();
	}
}
