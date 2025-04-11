namespace WebVella.Tefter.Assets.Addons;

public partial class AssetsAdminPage : TfBaseComponent, ITfAuxDataState, 
	ITfRegionComponent<TfAdminPageScreenRegionContext>
{
	public Guid Id { get; init; }
	public int PositionRank { get; init; }
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init; }
	public List<TfScreenRegionScope> Scopes { get; init; }
	[Parameter] 
	public TfAdminPageScreenRegionContext RegionContext { get; init; }

	public AssetsAdminPage() : base()
	{
		var componentId = new Guid("9cf13acf-8959-499e-aab8-ff2c25a6c97e");
		Id = componentId;
		PositionRank = 100;
		Name = "Assets Folders";
		Description = "";
		FluentIconName = "Folder";
		Scopes = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,componentId)
		};		
	}

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
		catch
		{
			newAuxDataState.Data[AssetsConstants.ASSETS_APP_SHARED_COLUMNS_LIST_DATA_KEY] = new List<TfSharedColumn>();
		}

        return Task.CompletedTask;
    }
}