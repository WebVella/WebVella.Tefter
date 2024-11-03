namespace WebVella.Tefter.Assets;

public class AssetsApp : ITfApplication
{
	public Guid Id => TfAssetConstants.ASSETS_APP_ID;

	public string Name => TfAssetConstants.ASSETS_APP_NAME;

	public string Description => TfAssetConstants.ASSETS_APP_DECRIPTION;

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<IAssetService, AssetService>();
	}

}
