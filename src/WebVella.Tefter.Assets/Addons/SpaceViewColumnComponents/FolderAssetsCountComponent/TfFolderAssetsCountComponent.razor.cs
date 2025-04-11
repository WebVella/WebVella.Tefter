using ClosedXML.Excel;

namespace WebVella.Tefter.Assets.Addons;

/// <summary>
/// Description attribute is needed when presenting the component to the user as a select option
/// Localization attributes is needed to strongly type the location of the components translation resource
/// </summary>
[Description("Folder Assets Count Display")]
[LocalizationResource("WebVella.Tefter.Assets.Addons.SpaceViewColumnComponents.FolderAssetsCountComponent.TfFolderAssetsCountComponent", "WebVella.Tefter.Assets")]
public partial class TfFolderAssetsCountComponent : TucBaseViewColumn<TfFolderAssetsCountComponentOptions>, ITfAuxDataState
{
	#region << Injects >>
	[Inject] protected IAssetsService AssetsService { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	#endregion

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TfFolderAssetsCountComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TfFolderAssetsCountComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		Context = context;
	}
	#endregion

	#region << Properties >>
	public override Guid Id { get; init; } = new Guid(AssetsConstants.ASSETS_ADDONS_SPACE_VIEW_COLUMN_COMPONENTS_FOLDER_ASSET_COUNT_ID);
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(AssetsConstants.ASSETS_ADDONS_SPACE_VIEW_COLUMN_TYPES_FOLDER_ASSET_COUNT_ID)
	};
	private List<TucSelectOption> _sharedKeyOptions = new();
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private Guid? _renderedHash = null;
	private string _storageKey = "";
	private IDialogReference _dialog;
	private List<AssetsFolder> _folders = new();
	private AssetsFolder _selectedFolder = null;
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initStorageKeys();
		if (Context.Mode == TfComponentPresentationMode.Options)
		{
			var _folders = AssetsService.GetFolders();
	
			if (componentOptions.FolderId is not null)
			{
				if (_folders.Count > 0)
				{
					var selectedIndex = _folders.FindIndex(x => x.Id == componentOptions.FolderId);
					if (selectedIndex == -1)
					{
						//This channel was probably deleted
						componentOptions.FolderId = null;
						await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), (Guid?)null);
					}
					else
					{
						_selectedFolder = _folders[selectedIndex];
					}
				}
			}
			else
			{
				if (_folders.Count > 0)
				{
					await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), _folders[0].Id);
					_selectedFolder = _folders[0];
				}
			}
		}
	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (Context.Hash != _renderedHash)
		{
			_initValues();
			_renderedHash = Context.Hash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override void ProcessExcelCell(IServiceProvider serviceProvider,IXLCell excelCell)
	{
		return;
	}

	public override Task OnAppStateInit(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		_initStorageKeys();
		var options = new List<TucSelectOption>();
		if (newAppState.SpaceView.SpaceDataId is not null)
		{
			var tfService = serviceProvider.GetRequiredService<ITfService>();
			var spaceData = tfService.GetSpaceData(newAppState.SpaceView.SpaceDataId.Value);
			if ( spaceData is not null)
			{
				var dataProvider = tfService.GetDataProvider(spaceData.DataProviderId);
				if (dataProvider is not null)
				{
					foreach (var key in dataProvider.SharedKeys)
					{
						options.Add(new TucSelectOption
						{
							Value = key.DbName,
							Label = key.DbName
						});
					}
				}
			}
		}
		newAuxDataState.Data[_storageKey] = options;
		return Task.CompletedTask;
	}
	#endregion
	#region << Private logic >>
	private async Task _onClick()
	{
		var panelContext = new AssetsFolderPanelContext
		{
			FolderId = componentOptions.FolderId,
			DataTable = Context.DataTable,
			RowIndex = Context.RowIndex
		};

		_dialog = await DialogService.ShowPanelAsync<AssetsFolderPanel>(
		panelContext,
		new DialogParameters()
		{
			DialogType = DialogType.Panel,
			Alignment = HorizontalAlignment.Right,
			ShowTitle = false,
			ShowDismiss = false,
			PrimaryAction = null,
			SecondaryAction = null,
			Width = "25vw",
			TrapFocus = false
		});
	}

	private void _initValues()
	{
		if (!TfAuxDataState.Value.Data.ContainsKey(_storageKey)) return;
		_sharedKeyOptions = ((List<TucSelectOption>)TfAuxDataState.Value.Data[_storageKey]).ToList();
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + Context.SpaceViewColumnId;
	}

	private async Task _folderSelectHandler(AssetsFolder folder)
	{
		_selectedFolder = folder;
		await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), folder?.Id);
	}
	#endregion

}

public class TfFolderAssetsCountComponentOptions
{
	public Guid? FolderId { get; set; } = null;
}