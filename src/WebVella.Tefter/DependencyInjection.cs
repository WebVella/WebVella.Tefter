namespace WebVella.Tefter;

public static class DependencyInjection
{
	public static IServiceCollection AddTefter(this IServiceCollection services, bool unitTestModeOn = false)
	{
		//because server render components are not disposed for about 10 min after page is left by browser
		//maybe 5 seconds is too low ???
		//https://stackoverflow.com/questions/78451698/dispose-is-never-called-for-any-server-side-blazor-components
		services.AddServerSideBlazor().AddCircuitOptions(options =>
		{
			options.DetailedErrors = true;
			options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(5);
		});

		///Adds localization for components
		services.AddLocalization();

		//Add Fluent UI which is our component library
		//https://www.fluentui-blazor.net/
		services.AddFluentUIComponents();

		services.AddScoped<IDataBroker, DataBroker>();
		services.AddScoped<ITfService, TfService>();
		services.AddSingleton<IWebConfigurationService, WebConfigurationService>();


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

	public static IServiceProvider UseTefter(this IServiceProvider serviceProvider)
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
