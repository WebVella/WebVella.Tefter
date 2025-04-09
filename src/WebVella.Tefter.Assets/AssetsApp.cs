namespace WebVella.Tefter.Assets;
public class AssetsApp : ITfApplicationAddon
{
	/// <summary>
	/// used as unique identifier
	/// </summary>
	public Guid Id { get; init; } = AssetsConstants.ASSETS_APP_ID;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Name { get; init; } = AssetsConstants.ASSETS_APP_NAME;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string Description { get; init; } = AssetsConstants.ASSETS_APP_DECRIPTION;
	/// <summary>
	/// presented to the end user
	/// </summary>
	public string FluentIconName { get; init; } = "Folder";
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
