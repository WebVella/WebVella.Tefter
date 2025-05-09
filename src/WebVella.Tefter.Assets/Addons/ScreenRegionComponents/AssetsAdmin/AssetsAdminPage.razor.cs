namespace WebVella.Tefter.Assets.Addons;

public partial class AssetsAdminPage : TfBaseComponent, ITfAuxDataState,
	ITfScreenRegionComponent<TfAdminPageScreenRegionContext>
{
	public const string ID = "9cf13acf-8959-499e-aab8-ff2c25a6c97e";
	public const string NAME = "Assets Folders";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "Folder";
	public const int POSITION_RANK = 100;

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
			new TfScreenRegionScope(null,new Guid(ID))
		};
	[Parameter]
	public TfAdminPageScreenRegionContext RegionContext { get; init; }


	public Task OnAppStateInit(IServiceProvider serviceProvider, TucUser currentUser,
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