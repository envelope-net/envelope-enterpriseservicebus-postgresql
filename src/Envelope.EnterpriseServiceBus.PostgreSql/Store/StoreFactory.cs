using Envelope.EnterpriseServiceBus.PostgreSql.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.Internal;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Store;

public static class StoreFactory
{
	public static Guid CreateStore(Action<PostgreSqlStoreConfigurationBuilder> configure)
	{
		if (configure == null)
			throw new ArgumentNullException(nameof(configure));

		var storeBuilder = new PostgreSqlStoreConfigurationBuilder();
		configure.Invoke(storeBuilder);
		var postgreSqlStoreConfiguration = storeBuilder.Build();
		StoreProvider.AddStore(postgreSqlStoreConfiguration);
		var storeKey = postgreSqlStoreConfiguration.StoreKey;
		return storeKey;
	}
}
