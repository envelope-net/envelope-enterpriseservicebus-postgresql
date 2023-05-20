using Envelope.EnterpriseServiceBus.PostgreSql.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Queries.Internal;
using Envelope.EnterpriseServiceBus.PostgreSql.Writers.Internal;
using Envelope.ServiceBus.Queries;
using Envelope.ServiceBus.Writers;
using Envelope.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Envelope.EnterpriseServiceBus.Extensions;

public static partial class ServiceCollectionExtensions
{
	private static readonly string _postgreSqlTransactionDocumentSessionCacheType = typeof(PostgreSqlTransactionDocumentSessionCache).FullName!;

	public static IServiceCollection AddServiceBusPostgreSql(
		this IServiceCollection services,
		Guid storeKey,
		ServiceLifetime serviceBusReaderLifetime = ServiceLifetime.Scoped)
	{
		services.TryAddTransient<ITransactionCoordinator, TransactionCoordinator>();
		services.TryAdd(new ServiceDescriptor(typeof(IServiceBusReader), sp => new ServiceBusReader(storeKey), serviceBusReaderLifetime));
		services.TryAddTransient<IJobMessagePublisher>(sp =>
		{
			var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
			return new JobMessageWriter(storeKey, loggerFactory.CreateLogger<JobMessageWriter>());
		});

		services.AddTransient<ITransactionCacheFactoryStore>(sp => new TransactionCacheFactoryStore(
			_postgreSqlTransactionDocumentSessionCacheType,
			serviceProvider =>
			{
				var store = StoreProvider.GetStore(storeKey);
				return new PostgreSqlTransactionDocumentSessionCache(store);
			}));

		return services;
	}

	public static IServiceCollection AddPostgreSqlJobMessageWriter(
		this IServiceCollection services,
		Guid storeKey,
		ServiceLifetime serviceBusWriterLifetime = ServiceLifetime.Scoped)
	{
		services.TryAdd(
			new ServiceDescriptor(
				typeof(IJobMessageWriter),
				sp =>
				{
					var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
					return new JobMessageWriter(storeKey, loggerFactory.CreateLogger<JobMessageWriter>());
				},
				serviceBusWriterLifetime));

		return services;
	}
}
