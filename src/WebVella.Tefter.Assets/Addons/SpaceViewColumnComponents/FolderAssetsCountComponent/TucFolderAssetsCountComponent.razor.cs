using ClosedXML.Excel;

namespace WebVella.Tefter.Assets.Addons;

public partial class TucFolderAssetsCountComponent : TucBaseViewColumn<TfFolderAssetsCountComponentOptions>
{
	public const string ID = "6c32d6e7-8758-4916-9685-e0476275a3a2";
	public const string NAME = "Folder Assets Count Display";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "";


	#region << Injects >>
	[Inject] protected IAssetsService AssetsService { get; set; }
	#endregion

	#region << Constructor >>
	/// <summary>
	/// Needed because of the custom constructor
	/// </summary>
	[ActivatorUtilitiesConstructor]
	public TucFolderAssetsCountComponent()
	{
	}

	/// <summary>
	/// The custom constructor is needed because in varoius cases we need to instance the component without
	/// rendering. The export to excel is one of those cases.
	/// </summary>
	/// <param name="context">this value contains options, the entire DataTable as well as the row index that needs to be processed</param>
	public TucFolderAssetsCountComponent(TfSpaceViewColumnScreenRegionContext context)
	{
		RegionContext = context;
	}
	#endregion

	#region << Properties >>
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = NAME;
	public override string AddonDescription { get; init; } = DESCRIPTION;
	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public override List<Guid> SupportedColumnTypes { get; init; } = new List<Guid>{
		new Guid(TfFolderAssetsCountViewColumnType.ID)
	};
	/// <summary>
	/// Each state has an unique hash and this is set in the component context under the Hash property value
	/// </summary>
	private string _renderedHash = null;
	private string _storageKey = "";
	private IDialogReference _dialog;
	private List<AssetsFolder> _folders = new();
	private AssetsFolder _selectedFolder = null;
	private long? _value = null;
	#endregion

	#region << Lifecycle >>
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_initStorageKeys();
		await _initValues();

	}

	/// <summary>
	/// When data needs to be inited, parameter set is the best place as Initialization is 
	/// done only once
	/// </summary>
	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var contextHash = RegionContext.GetHash();
		if (contextHash != _renderedHash)
		{
			await _initValues();
			_renderedHash = contextHash;
		}
	}
	#endregion

	#region << Non rendered methods >>
	/// <summary>
	/// Overrides the default export method in order to apply its own options
	/// </summary>
	/// <returns></returns>
	public override void ProcessExcelCell(IServiceProvider serviceProvider, IXLCell excelCell)
	{
		return;
	}

	#endregion
	#region << Private logic >>
	private async Task _onClick()
	{
		var panelContext = new AssetsFolderPanelContext
		{
			FolderId = componentOptions.FolderId,
			DataTable = RegionContext.DataTable,
			RowIndex = RegionContext.RowIndex
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
			TrapFocus = false,
			OnDialogClosing = EventCallback.Factory.Create<DialogInstance>(this, async (instance) =>
			{
				var change = ((AssetsFolderPanelContext)instance.Content).CountChange;
				_value = (_value ?? 0) + change;
				if (_value <= 0) _value = null;
				await InvokeAsync(StateHasChanged);
			})
		});
	}

	private async Task _initValues()
	{
		if (RegionContext.Mode == TfComponentPresentationMode.Options)
		{
			_folders = AssetsService.GetFolders();

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
					_selectedFolder = _folders[0];
				}
			}
			if (_selectedFolder is not null)
			{
				await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), _selectedFolder?.Id);
				await OnDataMappingChanged("Value", $"{_selectedFolder.DataIdentity}.{_selectedFolder.CountSharedColumnName}");
			}
			else{ 
				await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), (Guid?)null);
				await OnDataMappingChanged("Value", null);			
			}
		}
		else if (RegionContext.Mode == TfComponentPresentationMode.Display)
		{
			_selectedFolder = null;
			if (RegionContext.ViewData is not null
				&& RegionContext.ViewData.ContainsKey(_storageKey)
				&& RegionContext.ViewData[_storageKey] is AssetsFolder)
			{
				_selectedFolder = (AssetsFolder)RegionContext.ViewData[_storageKey];
			}
			else if (componentOptions.FolderId is not null)
			{
				_selectedFolder = AssetsService.GetFolder(componentOptions.FolderId.Value);
				RegionContext.ViewData[_storageKey] = _selectedFolder;
			}
			_value = null;
			if (_selectedFolder is not null
				&& !String.IsNullOrWhiteSpace(_selectedFolder.DataIdentity)
				&& !String.IsNullOrWhiteSpace(_selectedFolder.CountSharedColumnName))
			{
				var columnName = $"{_selectedFolder.DataIdentity}.{_selectedFolder.CountSharedColumnName}";
				_value = (long?)GetDataStruct<long>(columnName, null);
			}

		}
	}

	private void _initStorageKeys()
	{
		_storageKey = this.GetType().Name + "_" + RegionContext.SpaceViewColumnId;
	}

	private async Task _folderSelectHandler(AssetsFolder folder)
	{
		_selectedFolder = folder;
		await OnOptionsChanged(nameof(TfFolderAssetsCountComponentOptions.FolderId), folder?.Id);

		await OnDataMappingChanged("Value",$"{folder.DataIdentity}.{folder.CountSharedColumnName}");
	}
	#endregion

}

public class TfFolderAssetsCountComponentOptions
{
	public Guid? FolderId { get; set; } = null;
}