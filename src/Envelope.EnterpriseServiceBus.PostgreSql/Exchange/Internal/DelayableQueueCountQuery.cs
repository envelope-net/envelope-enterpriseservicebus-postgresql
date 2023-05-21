using Envelope.EnterpriseServiceBus.Messages;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages;
using Marten.Linq;
using System.Linq.Expressions;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;

public class DelayableQueueCountQuery : ICompiledQuery<DbExchangeMessage, int>
{
	public DateTime NowUtc { get; set; }

	public Expression<Func<IMartenQueryable<DbExchangeMessage>, int>> QueryIs()
	{
		return q => q
			.Count(x =>
				x.ExchangeMessage.MessageStatus != MessageStatus.Completed
				&& x.ExchangeMessage.MessageStatus != MessageStatus.Discarded
				&& x.ExchangeMessage.MessageStatus != MessageStatus.Aborted
				&& x.ExchangeMessage.MessageStatus != MessageStatus.Suspended
				&& (!x.ExchangeMessage.DelayedToUtc.HasValue
					|| x.ExchangeMessage.DelayedToUtc < NowUtc));
	}
}
