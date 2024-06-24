namespace WebVella.Tefter;

public static class DependencyInjection
{
	public static IServiceCollection AddTefterDI(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddHttpContextAccessor();
		services.AddCascadingAuthenticationState();
		services.AddAuthorizationCore();
		services.AddAuthenticationCore();

		services.AddScoped<AuthenticationStateProvider, TfAuthStateProvider>();
		services.AddSingleton<ILogger, NullLogger>();
		services.AddSingleton<IDbConfigurationService, DatabaseConfigurationService>((Context) =>
		{
			return new DatabaseConfigurationService(new ConfigurationBuilder()
				.AddJsonFile("appsettings.json".ToApplicationPath())
				.AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
		   .Build());
		});

		//messaging
		services.AddSingleton<IChannelEventRouter, ChannelEventRouter>();
		services.AddTransient<ITfEventBus, TfEventBus>();
		services.AddTransient<UserEventProvider, UserEventProvider>();
		services.AddTransient<GlobalEventProvider, GlobalEventProvider>();

		services.AddSingleton<ITfDataProviderManager, TfDataProviderManager>();
		services.AddSingleton<ICryptoService, CryptoService>();
		services.AddSingleton<ICryptoServiceConfiguration, CryptoServiceConfiguration>();
		services.AddSingleton<ITransactionRollbackNotifyService, TransactionRollbackNotifyService>();
		services.AddSingleton<IDatabaseService, DatabaseService>();
		services.AddSingleton<IDatabaseManager, DatabaseManager>();
		services.AddSingleton<IDboManager, DboManager>();
		services.AddSingleton<IMigrationManager, MigrationManager>();
		services.AddSingleton<IIdentityManager, IdentityManager>();


		return services;
	}

	public static IServiceProvider UseTefterDI(this IServiceProvider serviceProvider)
	{
		//because application domain does not get assemblies with no instances yet
		//we for load of all assemblies (its workaround)
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			var loadedPaths = loadedAssemblies.Select(a => a.Location).ToHashSet();
			var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
			var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
			toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));
		}

		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();
		return serviceProvider;
	}
}
