namespace WebVella.Tefter.Web;
using WebVella.Tefter;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddTefter(this IServiceCollection services, bool unitTestModeOn = false)
	{
		//register dependencies from WebVella.Tefter
		services.AddTefterDI(unitTestModeOn);

		services.AddSingleton<IConfigurationService, ConfigurationService>();
		services.AddScoped<IDataBroker, DataBroker>();
		services.AddScoped<ITfService, TfService>();
		return services;
	}

	public static IServiceProvider UseTefter(this IServiceProvider serviceProvider)
	{
		serviceProvider.UseTefterDI();
		return serviceProvider;
	}
}
