﻿using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Assets.Addons;
[LocalizationResource("WebVella.Tefter.Assets.Addons.SpacePages.AssetsPage.AssetsPageComponent", "WebVella.Tefter.Assets")]
public partial class AssetsSpacePageComponent : TucBaseSpacePageComponent
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
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = NAME;
	public override string AddonDescription { get; init; } = DESCRIPTION;
	public override string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
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

	public override async Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		await base.OnPageCreated(serviceProvider, context);
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) throw new Exception("AssetPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<AssetsSpacePageComponentOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("AssetPageComponent error: options cannot be deserialized");

		return context.ComponentOptionsJson;
	}
	#endregion

	#region << Private properties >>
	private string optionsJson = "{}";
	private AssetsSpacePageComponentOptions _options = new();
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
			_options = JsonSerializer.Deserialize<AssetsSpacePageComponentOptions>(optionsJson);
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
			_options = JsonSerializer.Deserialize<AssetsSpacePageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if (_options is null) _options = new();
			if (_options.FolderId is null && _optionsFolder is not null) _options.FolderId = _optionsFolder.Id;
		}
	}
	#endregion

	#region << Private methods >>

	private string _getJoinKeyValue()
	{
		switch (_options.JoinKeyType)
		{
			case AssetsFolderJoinKeyType.SpaceNodeId:
				return (TfAppState.Value.SpaceNode?.Id ?? Guid.Empty).ToString();
			case AssetsFolderJoinKeyType.SpaceId:
				return (TfAppState.Value.Space?.Id ?? Guid.Empty).ToString();
			case AssetsFolderJoinKeyType.CustomString:
				return (String.IsNullOrWhiteSpace(_options.CustomJoinKeyValue) ? Guid.Empty.ToString() : _options.CustomJoinKeyValue);
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

public class AssetsSpacePageComponentOptions
{
	[JsonPropertyName("FolderId")]
	public Guid? FolderId { get; set; } = null;

	[JsonPropertyName("ViewType")]
	public AssetsFolderViewType ViewType { get; set; } = AssetsFolderViewType.List;

	[JsonPropertyName("AssetId")]
	public Guid? AssetId { get; set; } = null;

	[JsonPropertyName("JoinKeyType")]
	public AssetsFolderJoinKeyType JoinKeyType { get; set; } = AssetsFolderJoinKeyType.SpaceNodeId;

	[JsonPropertyName("CustomJoinKeyValue")]
	public string CustomJoinKeyValue { get; set; } = "";
}