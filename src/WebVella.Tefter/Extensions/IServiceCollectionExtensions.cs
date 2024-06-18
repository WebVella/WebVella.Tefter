namespace WebVella.Tefter;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddTefterDI(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddSingleton<ILogger, NullLogger>();
		services.AddSingleton<IDbConfigurationService, DatabaseConfigurationService>((Context) =>
		{
			return new DatabaseConfigurationService(new ConfigurationBuilder()
				.AddJsonFile("appsettings.json".ToApplicationPath())
				.AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
		   .Build());
		});
		services.AddSingleton<ITransactionRollbackNotifyService, TransactionRollbackNotifyService>();
		services.AddSingleton<IDatabaseService, DatabaseService>();
		services.AddSingleton<IDatabaseManager, DatabaseManager>();
		services.AddSingleton<IDboManager, DboManager>();
		services.AddSingleton<IMigrationManager, MigrationManager>();

		return services;
	}

	public static IServiceProvider UseTefterDI(this IServiceProvider serviceProvider)
	{
		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();
		return serviceProvider;
	}
}
