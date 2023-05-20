using Envelope.EnterpriseServiceBus.Jobs.Configuration;
using Envelope.EnterpriseServiceBus.PostgreSql.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Jobs.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Jobs.Logging;
using Envelope.EnterpriseServiceBus.PostgreSql.Queries.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Writers.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Envelope.EnterpriseServiceBus.PostgreSql.Extensions;

public static class JobProviderConfigurationBuilderExtensions
{
	public static JobProviderConfigurationBuilder ConfigurePostgreSql(
		this JobProviderConfigurationBuilder builder,
		Guid storeKey)
	{
		if (builder == null)
			throw new ArgumentNullException(nameof(builder));

		if (storeKey == default)
			 storeKey = StoreProvider.DefaultStoreKey;

		builder
			.JobRepository(sp => new PostgreSqlJobRepository())
			.JobLogger(sp => new PostgreSqlJobLogger(
				storeKey,
				sp.GetRequiredService<IApplicationContext>(),
				sp.GetRequiredService<ILogger<PostgreSqlJobLogger>>()))
			.ServiceBusReader(sp => new ServiceBusReader(storeKey))
			.JobMessageWriter(sp => new JobMessageWriter(
				storeKey,
				//sp.GetRequiredService<IApplicationContext>(),
				sp.GetRequiredService<ILogger<JobMessageWriter>>()));

		return builder;
	}
}
