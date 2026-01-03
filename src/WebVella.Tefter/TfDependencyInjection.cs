namespace WebVella.Tefter;

public static class TfDependencyInjection
{
	public static IServiceCollection AddTefter(
		this IServiceCollection services,
		IConfigurationRoot? config = null)
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

		services.AddAuthentication(TfAuthSchemeOptions.DefaultScheme)
			.AddScheme<TfAuthSchemeOptions, TfAuthSchemeHandler>(
				TfAuthSchemeOptions.DefaultScheme,
				opts => { }
			);
		//services.AddTransient<TfAuthenticationMiddleware>();
		services.AddScoped<AuthenticationStateProvider, TfAuthStateProvider>();
		services.AddSingleton<ITfConfigurationService, TfConfigurationService>((Context) =>
		{
			var configBuilder = new ConfigurationBuilder();
			if(config is not null)
			{
				configBuilder.AddConfiguration(config);
			}
			else
			{
				configBuilder
					.AddJsonFile("appsettings.json".ToApplicationPath())
					.AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
					.AddEnvironmentVariables();
			}
			return new TfConfigurationService(configBuilder.Build());
		});

		//messaging
		services.AddSingleton<ITfEventBus, TfEventBus>();
		// services.AddSingleton<ITfChannelEventRouter, TfChannelEventRouter>();
		// services.AddTransient<ITfEventBus, TfEventBus>();
		// services.AddTransient<TfUserEventProvider, TfUserEventProvider>();
		// services.AddTransient<TfGlobalEventProvider, TfGlobalEventProvider>();

		services.AddSingleton<ITfCryptoService, TfCryptoService>();
		services.AddSingleton<ITfCryptoServiceConfiguration, TfCryptoServiceConfiguration>();
		services.AddSingleton<ITfTransactionRollbackNotifyService, TfTransactionRollbackNotifyService>();
		services.AddSingleton<ITfDbConfigurationService, TfDatabaseConfigurationService>();
		services.AddSingleton<ITfDatabaseService, TfDatabaseService>();
		services.AddSingleton<ITfDatabaseManager, TfDatabaseManager>();
		services.AddSingleton<ITfDboManager, TfDboManager>();
		services.AddSingleton<IMigrationManager, MigrationManager>();
		services.AddSingleton<ITfMetaService, TfMetaService>();
		services.AddSingleton<ITfService, TfService>();

		//hosted services
		services.AddHostedService<TfDataProviderSynchronizeJob>();
		services.AddHostedService<TfBlobMaintenanceJob>();
		services.AddHostedService<TfDataProviderSynchScheduleJob>();


		//we don't use static constructor here, 
		//because no control on assemblies order loading
		TfMetaService.Init();

		//inject classes from applications
		var applications = TfMetaService.GetApplications();
		foreach (var app in applications)
			app.OnRegisterDependencyInjections(services);

		if (config is not null)
		{
			if (bool.TryParse(config["Tefter:DisableBackgroundJobs"], out var disableBackgroundJobs))
			{
				if (disableBackgroundJobs)
				{
					// Remove all IHostedService registrations
					var hostedServiceDescriptors = services
						.Where(sd => sd.ServiceType == typeof(IHostedService))
						.ToList();

					foreach (var descriptor in hostedServiceDescriptors)
					{
						services.Remove(descriptor);
					}
				}
			}
		}

		return services;
	}

	internal static IServiceProvider UseTefter(this IServiceProvider serviceProvider)
	{
		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();

		//execute application on start methods
		var applications = TfMetaService.GetApplications();
		foreach (var app in applications)
			app.OnStart();

		return serviceProvider;
	}

	public static WebApplication UseTefter(this WebApplication webApp)
	{
		webApp.Services.UseTefter();
		//webApp.UseMiddleware<TfAuthenticationMiddleware>();
		return webApp;
	}

	private static void LoadAllAssemblies()
	{
		//because application domain does not get assemblies with no instances yet
		//we for load of all assemblies (its workaround)
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			var loadedPaths = loadedAssemblies.Select(a => a.Location).ToHashSet();
			var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
			var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase))
				.ToList();
			foreach (var path in toLoad)
			{
				Assembly assembly = null;
				try { assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)); }
				catch { }

				;
				if (assembly != null)
					loadedAssemblies.Add(assembly);
			}
		}
	}
}