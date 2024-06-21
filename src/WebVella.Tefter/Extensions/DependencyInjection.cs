using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter;

public static class DependencyInjection
{
	public static IServiceCollection AddTefterDI(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddHttpContextAccessor();
		services.AddCascadingAuthenticationState();
		services.AddAuthorizationCore();
		services.AddAuthenticationCore();

		//used by authentication
		//services.AddSingleton<TfTempStorageSessionStoreService>();
		//services
		//	.AddAuthentication(TfAuthenticationOptions.DefaultScheme)
		//	.AddScheme<TfAuthenticationOptions, TfAuthenticationHandler>(
		//		TfAuthenticationOptions.DefaultScheme, options => { });
		
		services.AddScoped<AuthenticationStateProvider, TfAuthStateProvider>();

		services.AddSingleton<ILogger, NullLogger>();
		services.AddSingleton<IDbConfigurationService, DatabaseConfigurationService>((Context) =>
		{
			return new DatabaseConfigurationService(new ConfigurationBuilder()
				.AddJsonFile("appsettings.json".ToApplicationPath())
				.AddJsonFile($"appsettings.{Environment.MachineName}.json".ToApplicationPath(), true)
		   .Build());
		});

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
		var migrationManager = serviceProvider.GetRequiredService<IMigrationManager>();
		migrationManager.CheckExecutePendingMigrationsAsync().Wait();
		return serviceProvider;
	}
}
