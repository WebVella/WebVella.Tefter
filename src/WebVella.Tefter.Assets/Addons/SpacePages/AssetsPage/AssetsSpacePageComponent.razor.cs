using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.Assets.Addons;
public partial class AssetsSpacePageComponent : TucBaseSpacePageComponent, IDisposable
{
	public const string ID = "0109d2c8-8597-47e7-bb4b-b1b06badd4a7";
	public const string NAME = "Asset Page";
	public const string DESCRIPTION = "asset folder per page";
	public const string FLUENT_ICON_NAME = "Folder";


	#region << Injects >>
	[Inject] protected IAssetsService AssetsService { get; set; }
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject] protected NavigationManager Navigator { get; set; } = null!;
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

	//OptionsMode
	private string optionsJson = "{}";
	private AssetsSpacePageComponentOptions _options = new();
	private AssetsFolder _optionsFolder = null;
	private List<Asset> _optionsFolderAssets = new();
	private List<AssetsFolder> _folders = new();

	//Read mode
	private TfUser _currentUser = new();
	private AssetsFolder _folder = null;
	private string _dataIdentityValue = null;
	private bool _isLoaded = false;
	private string _uriInitialized = string.Empty;

	#endregion


	#region << Render Lifecycle >>
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		_currentUser = TfAuthLayout.GetState().User;
		await _init(TfAuthLayout.GetState().NavigationState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		_isLoaded = true;
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
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (_uriInitialized != args.Location)
				await _init(navState: TfAuthLayout.GetState().NavigationState);
		});
	}
	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_options = null;
			if (!String.IsNullOrWhiteSpace(Context.ComponentOptionsJson))
			{
				optionsJson = Context.ComponentOptionsJson;
				_options = JsonSerializer.Deserialize<AssetsSpacePageComponentOptions>(optionsJson);
			}
			_dataIdentityValue = await _getDataIdentityValue();
			if (Context.Mode == TfComponentMode.Read)
			{
				_folder = null;
				if (_options?.FolderId is not null)
				{
					_folder = AssetsService.GetFolder(_options.FolderId.Value);
				}
			}
			else
			{
				_folders = AssetsService.GetFolders();
				_optionsFolder = null;
				if (_options?.FolderId is not null)
				{
					_optionsFolder = _folders.FirstOrDefault(x => x.Id == _options.FolderId);
				}
				if (_optionsFolder is null && _folders.Count > 0)
				{
					_optionsFolder = _folders[0];
					_options.FolderId = _optionsFolder.Id;
				}
			}
		}
		finally
		{
			_uriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task<string> _getDataIdentityValue()
	{
		if (_options is null) return Guid.NewGuid().ToSha1();
		var navState = TfAuthLayout.GetState().NavigationState;
		switch (_options.DataIdentityValueType)
		{
			case AssetsFolderDataIdentityValueType.SpacePageId:
				return (navState.SpacePageId ?? Guid.Empty).ToSha1();
			case AssetsFolderDataIdentityValueType.SpaceId:
				return (navState.SpaceId ?? Guid.Empty).ToSha1();
			case AssetsFolderDataIdentityValueType.CustomString:
				return String.IsNullOrWhiteSpace(_options.CustomDataIdentityValue) ? null : _options.CustomDataIdentityValue.ToSha1();
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

	[JsonPropertyName("DataIdentityValueType")]
	public AssetsFolderDataIdentityValueType DataIdentityValueType { get; set; } = AssetsFolderDataIdentityValueType.SpacePageId;

	[JsonPropertyName("CustomDataIdentityValue")]
	public string CustomDataIdentityValue { get; set; } = "";
}