namespace WebVella.Tefter.Web;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddTefter(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddSingleton<IConfigurationService, ConfigurationService>();
		services.AddScoped<IDataBroker, DataBroker>();
		services.AddScoped<ITfService, TfService>();
		return services;
	}
}
