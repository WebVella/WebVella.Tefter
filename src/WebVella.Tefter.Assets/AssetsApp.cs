namespace WebVella.Tefter.Assets;
public class AssetsApp : ITfApplication
{
	/// <summary>
	/// used as unique identifier
	/// </summary>
	public Guid Id => AssetsConstants.ASSETS_APP_ID;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Name => AssetsConstants.ASSETS_APP_NAME;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Description => AssetsConstants.ASSETS_APP_DECRIPTION;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string FluentIconName => "Folder";
	/// <summary>
	/// Called once on program start
	/// </summary>
	public void OnStart()
	{
	}
	/// <summary>
	/// Called during the service injection phase 
	/// </summary>
	/// <param name="services"></param>
	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<IAssetsService, AssetsService>();
	}

}
