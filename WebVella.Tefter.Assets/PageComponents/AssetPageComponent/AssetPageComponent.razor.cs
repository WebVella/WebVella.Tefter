﻿using WebVella.Tefter.Errors;

namespace WebVella.Tefter.Assets.PageComponents;
[LocalizationResource("WebVella.Tefter.Assets.PageComponents.AssetPageComponent.AssetPageComponent", "WebVella.Tefter.Assets")]
public partial class AssetPageComponent : TucBaseSpaceNodeComponent
{
	#region << Injects >>
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] protected IAssetsService AssetsService { get; set; }
	#endregion

	#region << Base Overrides >>
	public override Guid Id { get; set; } = new Guid("0109d2c8-8597-47e7-bb4b-b1b06badd4a7");
	public override string Name { get; set; } = "Asset Page";
	public override string Description { get; set; } = "asset folder per page";
	public override string Icon { get; set; } = "Folder";
	[Parameter] public override TfSpaceNodeComponentContext Context { get; set; }

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
		TfSpaceNodeComponentContext context)
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
	private string contextKeyInAuxDataState = "AssetsPageContext";
	private string optionsJson = "{}";
	private AssetsPageComponentPageComponentOptions _options { get; set; } = new();
	private AssetsFolder _optionsFolder { get; set; } = null;
	private List<AssetsFolder> _folders { get; set; } = new();

	#endregion


	#region << Render Lifecycle >>
	protected override void OnInitialized()
	{
		base.OnInitialized();
		var allFoldersResult = AssetsService.GetFolders();
		if (allFoldersResult.IsFailed) throw new Exception("GetFolders failed");
		_folders = allFoldersResult.Value;
		if (_folders.Count > 0)
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
			if(_options is null) _options = new();
			if(_options.FolderId is null && _optionsFolder is not null) _options.FolderId = _optionsFolder.Id;
		}
	}
	#endregion

	#region << Private methods >>


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
}

public class AssetsPageComponentPageComponentContext
{
	[JsonPropertyName("Folders")]
	public List<AssetsFolder> Folders { get; set; } = new();
}