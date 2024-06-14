namespace WebVella.Tefter.Web;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddTefter(this IServiceCollection services, bool unitTestModeOn = false)
	{
		services.AddScoped<IDataBroker, DataBroker>();
		services.AddScoped<ITfService, TfService>();
		return services;
	}
}
