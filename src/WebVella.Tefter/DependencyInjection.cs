using WebVella.Tefter.Api;

namespace WebVella.Tefter;

public static class DependencyInjection
{
	public static IServiceCollection AddTefter(this IServiceCollection services, bool unitTestModeOn = false)
	{
		LoadAllAssemblies();

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

		services.AddSingleton<ICryptoService, CryptoService>();
		services.AddSingleton<ICryptoServiceConfiguration, CryptoServiceConfiguration>();
		services.AddSingleton<ITransactionRollbackNotifyService, TransactionRollbackNotifyService>();
		services.AddSingleton<IDatabaseService, DatabaseService>();
		services.AddSingleton<IDatabaseManager, DatabaseManager>();
		services.AddSingleton<IDboManager, DboManager>();
		services.AddSingleton<IMigrationManager, MigrationManager>();
		services.AddSingleton<IIdentityManager, IdentityManager>();
		services.AddSingleton<IDataManager, DataManager>();
		services.AddSingleton<ITfSharedColumnsManager, TfSharedColumnsManager>();
		services.AddSingleton<ITfDataProviderManager, TfDataProviderManager>();
		services.AddSingleton<ITfSpaceManager, TfSpaceManager>();
		services.AddSingleton<ITfApplicationManager, TfApplicationManager>();
		services.AddSingleton<ITfScreenRegionComponentManager, TfScreenRegionComponentManager>();
		services.AddSingleton<ITfTypeProvider, TfTypeProvider>();


		//lazy services
		services.AddSingleton<Lazy<IDatabaseService>>(provider =>
				new Lazy<IDatabaseService>(() => provider.GetRequiredService<IDatabaseService>()));
		services.AddSingleton<Lazy<ITfSpaceManager>>(provider =>
				new Lazy<ITfSpaceManager>(() => provider.GetRequiredService<ITfSpaceManager>()));
		services.AddSingleton<Lazy<ITfDataProviderManager>>(provider =>
				new Lazy<ITfDataProviderManager>(() => provider.GetRequiredService<ITfDataProviderManager>()));

		//use cases
		services.AddTransient<AppStateUseCase, AppStateUseCase>();
		services.AddTransient<UserStateUseCase, UserStateUseCase>();
		services.AddTransient<ExportUseCase, ExportUseCase>();
		services.AddTransient<LoginUseCase, LoginUseCase>();

		//hosted services
		services.AddHostedService<TfDataProviderSynchronizeJob>();

		//we don't use static constructor here, 
		//because no control on assemblies order loading
		TfApplicationManager.Init();

		//inject classes from applications
		var applications = TfApplicationManager.GetApplicationsInternal();
		foreach (var app in applications)
			app.OnRegisterDependencyInjections(services);

		return services;
	}

	public static IServiceProvider UseTefter(this IServiceProvider serviceProvider)
	{
		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();

		//execute application on start methods
		var applications = TfApplicationManager.GetApplicationsInternal();
		foreach (var app in applications)
			app.OnStart();

		return serviceProvider;
	}

	private static void LoadAllAssemblies()
	{
		//because application domain does not get assemblies with no instances yet
		//we for load of all assemblies (its workaround)
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			var loadedPaths = loadedAssemblies.Select(a => a.Location).ToHashSet();
			var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
			var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
			foreach (var path in toLoad)
			{
				Assembly assembly = null;
				try { assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));  }catch {};
				if(assembly != null)
				loadedAssemblies.Add(assembly);
			}
		}
	}
}
