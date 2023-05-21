using Envelope.DependencyInjection;
using Envelope.EnterpriseServiceBus.PostgreSql.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.Store;
using Microsoft.Extensions.DependencyInjection;

namespace Envelope.EnterpriseServiceBus.PostgreSql;

public delegate void ConfigureEnterpriseServiceBusDb(PostgreSqlStoreConfigurationBuilder builder);

public class EnterpriseServiceBusDbStartupTask : IStartupTask
{
	public Task ExecuteAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
	{
		var storeConfigure = serviceProvider.GetRequiredService<ConfigureEnterpriseServiceBusDb>();
		StoreFactory.CreateStore(storeConfigure);

		return Task.CompletedTask;
	}
}
