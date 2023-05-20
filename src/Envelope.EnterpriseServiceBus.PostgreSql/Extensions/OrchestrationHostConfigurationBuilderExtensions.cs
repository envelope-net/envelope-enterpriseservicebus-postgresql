using Envelope.EnterpriseServiceBus.Orchestrations.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.DistributedCoordinator.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Orchestrations.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Orchestrations.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Extensions;

public static class OrchestrationHostConfigurationBuilderExtensions
{
	public static OrchestrationHostConfigurationBuilder ConfigurePostgreSql(
		this OrchestrationHostConfigurationBuilder builder,
		Guid storeKey)
	{
		if (builder == null)
			throw new ArgumentNullException(nameof(builder));

		if (storeKey == default)
			 storeKey = StoreProvider.DefaultStoreKey;

		builder
			.OrchestrationRepositoryFactory((sp, registry) => new PostgreSqlOrchestrationRepository(registry))
			.DistributedLockProviderFactory(sp => new PostgreSqlLockProvider(storeKey))
			.OrchestrationLogger(sp => new PostgreSqlOrchestrationLogger(storeKey, sp.GetRequiredService<ILogger<PostgreSqlOrchestrationLogger>>()));

		return builder;
	}
}
