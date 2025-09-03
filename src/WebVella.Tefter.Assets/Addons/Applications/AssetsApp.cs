namespace WebVella.Tefter.Assets.Addons;
public class AssetsApp : ITfApplicationAddon
{
	public const string ID = "5d229b2b-5c78-48fb-b91f-6e853f24aaf2";
	public const string NAME = "Talk Application";
	public const string DESCRIPTION = "Talk Application Description";
	public const string FLUENT_ICON_NAME = "Folder";

	public Guid AddonId { get; init;} =  new Guid(ID);
	public string AddonName { get; init;} =  NAME;
	public string AddonDescription { get; init;} =  DESCRIPTION;
	public string AddonFluentIconName { get; init;} =  FLUENT_ICON_NAME;
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
		//services.AddHostedService<AssetBackgroundJob>();
    }

}
