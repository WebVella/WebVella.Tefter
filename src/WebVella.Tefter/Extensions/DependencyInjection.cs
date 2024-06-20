using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebVella.Tefter;

public static class DependencyInjection
{
	public static IServiceCollection AddTefterDI(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		   .AddCookie(options =>
		   {
			   options.Cookie.HttpOnly = true;
			   options.Cookie.Name = "tefter";
			   options.LoginPath = new PathString("/login");
			   options.LogoutPath = new PathString("/logout");
			   options.AccessDeniedPath = new PathString("/error?access_denied");
			   options.ReturnUrlParameter = "ret_url";
		   });

		services.AddHttpContextAccessor();
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
