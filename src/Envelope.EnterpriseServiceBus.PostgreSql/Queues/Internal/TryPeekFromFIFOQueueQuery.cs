using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Queues.Internal;

public class TryPeekFromFIFOQueueQuery : ICompiledQuery<DbQueuedMessage, DbQueuedMessage?>
{
	public Expression<Func<IMartenQueryable<DbQueuedMessage>, DbQueuedMessage?>> QueryIs()
	{
		return q => q.OrderBy(x => x.QueuedMessage.PublishingTimeUtc).FirstOrDefault();
	}
}
