namespace WebVella.Tefter.Assets.Pages;

public partial class AssetsAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
	public Guid Id { get { return new Guid("9cf13acf-8959-499e-aab8-ff2c25a6c97e"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 10; } }
	public string Name { get { return "Assets Folders"; } }
	public string FluentIconName => "Folder";

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var assetsService = serviceProvider.GetRequiredService<IAssetsService>();
        var sharedColumnsManager = serviceProvider.GetRequiredService<ITfSharedColumnsManager>();
        var srvResult = assetsService.GetFolders();
        if (srvResult.IsSuccess)
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = srvResult.Value.OrderBy(x => x.Name).ToList();
        else
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = new List<AssetsFolder>();

        var sharedColumnsResult = sharedColumnsManager.GetSharedColumns();
        if (sharedColumnsResult.IsSuccess)
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumnsResult.Value.OrderBy(x => x.DbName).ToList();
        else
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();

        return Task.CompletedTask;
    }
}