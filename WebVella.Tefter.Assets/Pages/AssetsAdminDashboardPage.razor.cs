namespace WebVella.Tefter.Assets.Pages;

public partial class AssetsAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 2; } }
	public string Name { get { return "Assets Administration"; } }
	public string UrlSlug { get { return "assets-dashboard"; } }

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
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = new List<Folder>();

        var sharedColumnsResult = sharedColumnsManager.GetSharedColumns();
        if (sharedColumnsResult.IsSuccess)
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumnsResult.Value.OrderBy(x => x.DbName).ToList();
        else
            newAuxDataState.Data[TfAssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();

        return Task.CompletedTask;
    }
}