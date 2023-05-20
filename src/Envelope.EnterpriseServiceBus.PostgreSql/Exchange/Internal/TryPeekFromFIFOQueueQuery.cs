using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class TryPeekFromFIFOQueueQuery : ICompiledQuery<DbExchangeMessage, DbExchangeMessage?>
{
	public Expression<Func<IMartenQueryable<DbExchangeMessage>, DbExchangeMessage?>> QueryIs()
	{
		return q => q.OrderBy(x => x.ExchangeMessage.PublishingTimeUtc).FirstOrDefault();
	}
}
