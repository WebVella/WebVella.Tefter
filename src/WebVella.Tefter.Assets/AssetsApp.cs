namespace WebVella.Tefter.Assets;

public class AssetsApp : ITfApplication
{
	public Guid Id => TfAssetsConstants.ASSETS_APP_ID;

	public string Name => TfAssetsConstants.ASSETS_APP_NAME;

	public string Description => TfAssetsConstants.ASSETS_APP_DECRIPTION;
	public string FluentIconName => "Folder";

	public void OnStart()
	{
	}

	public void OnRegisterDependencyInjections(IServiceCollection services)
	{
		services.AddSingleton<IAssetsService, AssetsService>();
	}

}
