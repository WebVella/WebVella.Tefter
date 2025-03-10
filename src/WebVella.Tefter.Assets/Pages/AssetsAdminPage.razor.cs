namespace WebVella.Tefter.Assets.Pages;

public partial class AssetsAdminPage : TfBaseComponent, ITucAuxDataUseComponent, ITfRegionComponent<TfAdminPageComponentContext>
{
	public Guid Id { get; init; } = new Guid("9cf13acf-8959-499e-aab8-ff2c25a6c97e"); 
	public int PositionRank { get; init; } = 100;
	public string Name { get; init;} = "Assets Folders";
	public string Description { get; init;} = "";
	public string FluentIconName { get; init; } =  "Folder";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){
		new TfRegionComponentScope(null,new Guid("9cf13acf-8959-499e-aab8-ff2c25a6c97e"))
	};
	[Parameter] 
	public TfAdminPageComponentContext Context { get; init; }

	public Task OnAppStateInit(IServiceProvider serviceProvider,TucUser currentUser,
        TfAppState newAppState,
        TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
    {
        var assetsService = serviceProvider.GetRequiredService<IAssetsService>();
        var tfService = serviceProvider.GetRequiredService<ITfService>();

		try
		{
			var folders = assetsService.GetFolders();
			newAuxDataState.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = folders.OrderBy(x => x.Name).ToList();
		}
		catch
		{
			newAuxDataState.Data[AssetsConstants.ASSETS_APP_FOLDER_LIST_DATA_KEY] = new List<AssetsFolder>();
		}

		try
		{
			var sharedColumns = tfService.GetSharedColumns();
			newAuxDataState.Data[AssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = sharedColumns.OrderBy(x => x.DbName).ToList();
		}
		catch (Exception ex)
		{
			newAuxDataState.Data[AssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();
		}

        return Task.CompletedTask;
    }
}