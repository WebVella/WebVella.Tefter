using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Assets.Addons;
[LocalizationResource("WebVella.Tefter.Assets.Addons.SpacePages.AssetPage.AssetPageComponent", "WebVella.Tefter.Assets")]
public partial class AssetPageComponent : TucBaseSpaceNodeComponent
{
	public const string ID = "0109d2c8-8597-47e7-bb4b-b1b06badd4a7";
	public const string NAME = "Asset Page";
	public const string DESCRIPTION = "asset folder per page";
	public const string FLUENT_ICON_NAME = "Folder";


	#region << Injects >>
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] protected IAssetsService AssetsService { get; set; }
	#endregion

	#region << Base Overrides >>
	public override Guid Id { get; init; } = new Guid(ID);
	public override string Name { get; init; } = NAME;
	public override string Description { get; init; } = DESCRIPTION;
	public override string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	[Parameter] public override TfSpacePageAddonContext Context { get; set; }

	public override string GetOptions() => JsonSerializer.Serialize(_options);
	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();

		if (_options.FolderId is null)
		{
			ValidationErrors.Add(new ValidationError(nameof(_options.FolderId), "required"));
		}

		return ValidationErrors;
	}

	public override Task<(TfAppState, TfAuxDataState)> InitState(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState,
		TfSpacePageAddonContext context)
	{
		//var talkService = serviceProvider.GetService<ITalkService>();
		//var allFoldersResult = talkService.GetFolders();
		//if(allFoldersResult.IsFailed) throw new Exception("GetFolders failed");

		//newAuxDataState.Data[contextKeyInAuxDataState] = new TalkPageComponentPageComponentContext{ 
		//	TalkFolders = allFoldersResult.Value
		//};
		return Task.FromResult((newAppState, newAuxDataState));
	}
	#endregion

	#region << Private properties >>
	private string optionsJson = "{}";
	private AssetsPageComponentPageComponentOptions _options = new();
	private AssetsFolder _optionsFolder = null;
	private List<Asset> _optionsFolderAssets = new();
	private List<AssetsFolder> _folders = new();

	#endregion


	#region << Render Lifecycle >>
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_folders = AssetsService.GetFolders();
		if (!String.IsNullOrWhiteSpace(Context.ComponentOptionsJson))
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<AssetsPageComponentPageComponentOptions>(optionsJson);
			if (_options.FolderId is not null)
			{
				_optionsFolder = _folders.FirstOrDefault(x => x.Id == _options.FolderId);
			}
		}
		if (_optionsFolder is null && _folders.Count > 0)
		{
			_optionsFolder = _folders[0];
			_options.FolderId = _optionsFolder.Id;
		}
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<AssetsPageComponentPageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if (_options is null) _options = new();
			if (_options.FolderId is null && _optionsFolder is not null) _options.FolderId = _optionsFolder.Id;
		}
	}
	#endregion

	#region << Private methods >>

	private string _getSharedKeyValue()
	{
		switch (_options.ShareKeyType)
		{
			case AssetsFolderShareKeyType.SpaceNodeId:
				return (TfAppState.Value.SpaceNode?.Id ?? Guid.Empty).ToString();
			case AssetsFolderShareKeyType.SpaceId:
				return (TfAppState.Value.Space?.Id ?? Guid.Empty).ToString();
			case AssetsFolderShareKeyType.CustomString:
				return (String.IsNullOrWhiteSpace(_options.CustomShareKeyValue) ? Guid.Empty.ToString() : _options.CustomShareKeyValue);
			default:
				throw new Exception("Shared Key Type not supported");
		}
	}

	private Task _optionsFolderChangeHandler(AssetsFolder folder)
	{
		_optionsFolder = folder;
		_options.FolderId = folder?.Id;
		return Task.CompletedTask;
	}

	#endregion
}

public class AssetsPageComponentPageComponentOptions
{
	[JsonPropertyName("FolderId")]
	public Guid? FolderId { get; set; } = null;

	[JsonPropertyName("ViewType")]
	public AssetsFolderViewType ViewType { get; set; } = AssetsFolderViewType.List;

	[JsonPropertyName("AssetId")]
	public Guid? AssetId { get; set; } = null;

	[JsonPropertyName("ShareKeyType")]
	public AssetsFolderShareKeyType ShareKeyType { get; set; } = AssetsFolderShareKeyType.SpaceNodeId;

	[JsonPropertyName("CustomShareKeyValue")]
	public string CustomShareKeyValue { get; set; } = "";
}