using WebVella.Tefter.Services;

namespace WebVella.Tefter;

public static class TfDependencyInjection
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
		services.AddHttpClient();
		//Add Fluent UI which is our component library
		//https://www.fluentui-blazor.net/
		services.AddFluentUIComponents();

		services.AddHttpContextAccessor();
		services.AddCascadingAuthenticationState();
		services.AddAuthorizationCore();
		services.AddAuthenticationCore();

		services.AddScoped<AuthenticationStateProvider, TfAuthStateProvider>();
		services.AddSingleton<ILogger, NullLogger>();
		services.AddSingleton<ITfConfigurationService, TfConfigurationService>((Context) =>
		{
			return new TfConfigurationService(new ConfigurationBuilder()
				.AddJsonFile("appsettings.json".ToApplicationPath())
				.AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
		   .Build());
		});

		//messaging
		services.AddSingleton<ITfChannelEventRouter, TfChannelEventRouter>();
		services.AddTransient<ITfEventBus, TfEventBus>();
		services.AddTransient<TfUserEventProvider, TfUserEventProvider>();
		services.AddTransient<TfGlobalEventProvider, TfGlobalEventProvider>();

		services.AddSingleton<ITfCryptoService, TfCryptoService>();
		services.AddSingleton<ITfCryptoServiceConfiguration, TfCryptoServiceConfiguration>();
		services.AddSingleton<ITfTransactionRollbackNotifyService, TfTransactionRollbackNotifyService>();
		services.AddSingleton<ITfDbConfigurationService, TfDatabaseConfigurationService>();
		services.AddSingleton<ITfDatabaseService, TfDatabaseService>();
		services.AddSingleton<ITfDatabaseManager, TfDatabaseManager>();
		services.AddSingleton<ITfDboManager, TfDboManager>();
		services.AddSingleton<IMigrationManager, MigrationManager>();
		services.AddSingleton<IIdentityManager, IdentityManager>();
		services.AddSingleton<ITfDataManager, TfDataManager>();
		services.AddSingleton<ITfSharedColumnsManager, TfSharedColumnsManager>();
		services.AddSingleton<ITfDataProviderManager, TfDataProviderManager>();
		services.AddSingleton<ITfSpaceManager, TfSpaceManager>();
		services.AddSingleton<ITfMetaProvider, TfMetaProvider>();
		//services.AddSingleton<ITfFileManager, TfFileManager>();
		services.AddSingleton<ITfRepositoryService, TfRepositoryService>();
		services.AddSingleton<ITfBlobManager, TfBlobManager>();
		services.AddSingleton<ITfTemplateService, TfTemplateService>();


		//lazy services
		services.AddSingleton<Lazy<ITfDatabaseService>>(provider =>
				new Lazy<ITfDatabaseService>(() => provider.GetRequiredService<ITfDatabaseService>()));
		services.AddSingleton<Lazy<ITfSpaceManager>>(provider =>
				new Lazy<ITfSpaceManager>(() => provider.GetRequiredService<ITfSpaceManager>()));
		services.AddSingleton<Lazy<ITfDataProviderManager>>(provider =>
				new Lazy<ITfDataProviderManager>(() => provider.GetRequiredService<ITfDataProviderManager>()));

		//use cases
		services.AddTransient<AppStateUseCase, AppStateUseCase>();
		services.AddTransient<AppStateUseCase, AppStateUseCase>();
		services.AddTransient<UserStateUseCase, UserStateUseCase>();
		services.AddTransient<ExportUseCase, ExportUseCase>();
		services.AddTransient<LoginUseCase, LoginUseCase>();

		//hosted services
		services.AddHostedService<TfDataProviderSynchronizeJob>();

		//we don't use static constructor here, 
		//because no control on assemblies order loading
		TfMetaProvider.Init();

		//inject classes from applications
		var applications = TfMetaProvider.GetApplications();
		foreach (var app in applications)
			app.OnRegisterDependencyInjections(services);

		return services;
	}

	public static IServiceProvider UseTefter(this IServiceProvider serviceProvider)
	{
		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();

		//execute application on start methods
		var applications = TfMetaProvider.GetApplications();
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
				try { assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)); } catch { };
				if (assembly != null)
					loadedAssemblies.Add(assembly);
			}
		}
	}
}
