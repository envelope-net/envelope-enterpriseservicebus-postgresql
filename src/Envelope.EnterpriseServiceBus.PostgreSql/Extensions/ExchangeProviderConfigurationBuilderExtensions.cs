using Envelope.EnterpriseServiceBus.Exchange;
using Envelope.EnterpriseServiceBus.Exchange.Configuration;
using Envelope.EnterpriseServiceBus.Exchange.Routing.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages.Internal;
using Envelope.ServiceBus.Messages;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Extensions;

public static class ExchangeProviderConfigurationBuilderExtensions
{
	public static ExchangeProviderConfigurationBuilder RegisterPostgreSqlExchange<TMessage>(
		this ExchangeProviderConfigurationBuilder builder,
		Action<ExchangeRouterBuilder>? configureBindings = null,
		bool force = true)
		where TMessage : class, IMessage
		=> builder.RegisterExchange(
			typeof(TMessage).FullName!,
			sp =>
			{
				var exchangeConfiguration = ExchangeConfigurationBuilder<TMessage>
					.GetDefaultBuilder(builder.Internal().ServiceBusOptions, configureBindings)
					.FIFOQueue((sp, maxSize) => new DbExchangeMessageQueue<TMessage>(true))
					.DelayableQueue((sp, maxSize) => new DbExchangeMessageQueue<TMessage>(false))
					.MessageBodyProvider(sp => new PostgreSqlMessageBodyProvider())
					.Build();

				var context = new ExchangeContext<TMessage>(exchangeConfiguration);
				return new Exchange<TMessage>(context);
			},
			force);
}
