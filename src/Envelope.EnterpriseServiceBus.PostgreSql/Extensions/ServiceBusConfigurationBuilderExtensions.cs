using Envelope.EnterpriseServiceBus.Configuration;
using Envelope.EnterpriseServiceBus.Orchestrations.EventHandlers;
using Envelope.EnterpriseServiceBus.Orchestrations.Model;
using Envelope.EnterpriseServiceBus.PostgreSql.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.Exchange.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Hosts.Logging;
using Envelope.EnterpriseServiceBus.PostgreSql.MessageHandlers.Logging;
using Envelope.EnterpriseServiceBus.PostgreSql.Messages.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Queues.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Extensions;

public static class ServiceBusConfigurationBuilderExtensions
{
	public static ServiceBusConfigurationBuilder ConfigurePostgreSql(
		this ServiceBusConfigurationBuilder builder,
		Action<PostgreSqlStoreConfigurationBuilder> configure)
	{
		if (builder == null)
			throw new ArgumentNullException(nameof(builder));
		if (configure == null)
			throw new ArgumentNullException(nameof(configure));

		var storeKey = StoreFactory.CreateStore(configure);

		builder
			.OrchestrationEventsFaultQueue(sp => new PostgreSqlFaultQueue())
			.OrchestrationExchange(x => x
				.FIFOQueue((sp, maxSize) => new DbExchangeMessageQueue<OrchestrationEvent>(true))
				.DelayableQueue((sp, maxSize) => new DbExchangeMessageQueue<OrchestrationEvent>(false))
				.MessageBodyProvider(sp => new PostgreSqlMessageBodyProvider()))
			.OrchestrationQueue(x => x
				.FIFOQueue((sp, maxSize) => new DbMessageQueue<OrchestrationEvent>(true))
				.DelayableQueue((sp, maxSize) => new DbMessageQueue<OrchestrationEvent>(false))
				.MessageBodyProvider(sp => new PostgreSqlMessageBodyProvider())
				.MessageHandler((sp, options) => OrchestrationEventHandler.HandleMessageAsync))
			.HostLogger(sp => new PostgreSqlHostLogger(
				storeKey,
				sp.GetRequiredService<IApplicationContext>(),
				sp.GetRequiredService<ILogger<PostgreSqlHostLogger>>()))

			.HandlerLogger(sp => new PostgreSqlHandlerLogger(storeKey, sp.GetRequiredService<ILogger<PostgreSqlHandlerLogger>>()));

		return builder;
	}
}
