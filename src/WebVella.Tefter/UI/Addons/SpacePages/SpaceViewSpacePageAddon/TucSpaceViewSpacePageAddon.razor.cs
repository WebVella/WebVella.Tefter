namespace WebVella.Tefter.UI.Addons;

[LocalizationResource("WebVella.Tefter.Web.Addons.SpacePages.SpaceViewSpacePageAddon.TfSpaceViewSpacePageAddon", "WebVella.Tefter")]
public partial class TucSpaceViewSpacePageAddon : TucBaseSpacePageComponent
{
	#region << Render Injects >>
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] protected NavigationManager Navigator { get; set; } = default!;
	#endregion

	#region << Base Overrides >>
	public static string ID = "68afeecc-6ca9-4102-831d-ef4028057128";
	public override Guid AddonId { get; init; } = new Guid(ID);
	public override string AddonName { get; init; } = "Space View";
	public override string AddonDescription { get; init; } = "present data in a grid format";
	public override string AddonFluentIconName { get; init; } = "Table";
	[Parameter] public override TfSpacePageAddonContext Context { get; set; } = default!;

	public override string GetOptions() => JsonSerializer.Serialize(_options);
	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();
		if (_options.SetType == TfSpaceViewSetType.New)
		{
			if (String.IsNullOrWhiteSpace(_options.Name))
			{
				ValidationErrors.Add(new ValidationError(nameof(_options.Name), "required"));
			}
			if (_options.DataSetType == TfSpaceViewDataSetType.New)
			{
				if (String.IsNullOrWhiteSpace(_options.NewSpaceDataName))
				{
					ValidationErrors.Add(new ValidationError(nameof(_options.NewSpaceDataName), "required"));
				}
				if (_options.DataProviderId is null)
				{
					ValidationErrors.Add(new ValidationError(nameof(_options.DataProviderId), "required"));
				}
			}
			else
			{
				if (_options.SpaceDataId is null)
				{
					ValidationErrors.Add(new ValidationError(nameof(_options.SpaceDataId), "required"));
				}
			}
		}
		else
		{
			if (_options.SpaceViewId is null)
			{
				ValidationErrors.Add(new ValidationError(nameof(_options.SpaceViewId), "required"));
			}
		}

		return ValidationErrors;
	}

	//public override Task<(TfAppState, TfAuxDataState)> InitState(
	//	IServiceProvider serviceProvider,
	//	TucUser currentUser,
	//	TfAppState newAppState, TfAppState oldAppState,
	//	TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState,
	//	TfSpacePageAddonContext context)
	//{
	//	#region << Init Options >>
	//	if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) return Task.FromResult((newAppState, newAuxDataState)); ;

	//	var options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(context.ComponentOptionsJson);
	//	if (options is null || options.SpaceViewId is null) return Task.FromResult((newAppState, newAuxDataState)); ;
	//	#endregion

	//	#region << Init SpaceView>>
	//	newAppState = newAppState with { Route = newAppState.Route with { SpaceViewId = options.SpaceViewId } };
	//	#endregion


	//	return Task.FromResult((newAppState, newAuxDataState));
	//}

	public override async Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		await base.OnPageCreated(serviceProvider, context);
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) throw new Exception("TfSpaceViewPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("TfSpaceViewPageComponent error: options cannot be deserialized");
		Guid? originalSpaceViewId = jsonOptions.SpaceViewId;
		var tfService = serviceProvider.GetService<ITfService>();
		if (jsonOptions.SetType == TfSpaceViewSetType.New)
		{
			//Create view if needed
			var spaceView = new TfCreateSpaceViewExtended
			{
				Id = Guid.NewGuid(),
				AddDataSetColumns = jsonOptions.AddDataSetColumns,
				AddProviderColumns = jsonOptions.AddProviderColumns,
				SpaceDataId = jsonOptions.SpaceDataId,
				AddSharedColumns = jsonOptions.AddSharedColumns,
				AddSystemColumns = jsonOptions.AddSystemColumns,
				DataProviderId = jsonOptions.DataProviderId,
				DataSetType = jsonOptions.DataSetType,
				Name = jsonOptions.Name,
				NewSpaceDataName = jsonOptions.NewSpaceDataName,
				Position = 1,
				Presets = new List<TfSpaceViewPreset>(),
				Settings = new TfSpaceViewSettings(),
				SpaceId = context.SpaceId,
				Type = jsonOptions.Type,
			};

			var createdSpaceView = TfSpaceViewUIService.CreateSpaceView(spaceView);
			jsonOptions.SpaceViewId = createdSpaceView.Id;
			jsonOptions.SpaceDataId = createdSpaceView.SpaceDataId;
			context.ComponentOptionsJson = JsonSerializer.Serialize(jsonOptions);
		}
		return context.ComponentOptionsJson;
	}

	#endregion

	#region << Private properties >>
	//Edit
	private string optionsJson = "{}";
	private TfSpaceViewSpacePageAddonOptions _options { get; set; } = new();
	private TfDataProvider? _optionsDataProvider = null;
	private TfSpaceData? _optionsDataset = null;
	private TfSpaceView? _optionsExistingSpaceView = null;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 10;

	private ReadOnlyCollection<TfDataProvider> _allDataProviders = default!;
	private List<TfSpaceView> _allSpaceView = new();
	private List<TfSpaceData> _allSpaceData = new();

	#endregion


	#region << Render Lifecycle >>

	protected override void OnInitialized()
	{
		_allSpaceView = TfSpaceViewUIService.GetAllSpaceViews(Context.SpaceId);
		_allSpaceData = TfSpaceDataUIService.GetAllSpaceData(Context.SpaceId);
		_allDataProviders = TfDataProviderUIService.GetDataProviders();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Mode == TfComponentMode.Read) return;
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(optionsJson);

			//When cannot node has json from another page type
			if (_options is null) _options = new TfSpaceViewSpacePageAddonOptions();

			if (_options.SpaceViewId is not null)
				_optionsExistingSpaceView = _allSpaceView.FirstOrDefault(x => x.Id == _options.SpaceViewId.Value);
			if (_optionsExistingSpaceView is not null)
				_optionsDataset = _allSpaceData.FirstOrDefault(x => x.Id == _optionsExistingSpaceView.SpaceDataId);
			if (_options.DataProviderId is not null)
				_optionsDataProvider = _allDataProviders.FirstOrDefault(x => x.Id == _options.DataProviderId.Value);
			_generatedColumnsListInit();
		}
	}
	#endregion

	#region << Private methods >>
	private void _optionsSetTypeChangeHandler(TfSpaceViewSetType type)
	{
		_options.SetType = type;
		if (type == TfSpaceViewSetType.Existing)
		{
			if (_optionsExistingSpaceView is null && _allSpaceView.Any())
			{
				_optionsExistingSpaceView = _allSpaceView[0];
				_options.SpaceViewId = _optionsExistingSpaceView.Id;
			}
		}
	}
	private void _optionsDataSetTypeChangeHandler(TfSpaceViewDataSetType type)
	{
		_optionsDataProvider = null;
		_options.DataProviderId = null;
		_optionsDataset = null;
		_options.SpaceDataId = null;
		_options.DataSetType = type;

		if (type == TfSpaceViewDataSetType.New)
		{
			if (_allDataProviders.Any())
			{
				_options.DataProviderId = _allDataProviders[0].Id;
				_optionsDataProvider = _allDataProviders[0];
			}
		}
		else if (type == TfSpaceViewDataSetType.Existing)
		{
			var allSpaceData = TfSpaceDataUIService.GetAllSpaceData();
			if (allSpaceData.Any())
			{
				_options.SpaceDataId = allSpaceData[0].Id;
				_optionsDataset = allSpaceData[0];
			}
		}
		_generatedColumnsListInit();
	}

	private void _optionsDataProviderSelectedHandler(TfDataProvider provider)
	{
		_optionsDataProvider = null;
		_options.DataProviderId = null;

		if (provider is null) return;
		_optionsDataProvider = provider;
		_options.DataProviderId = provider.Id;
		_generatedColumnsListInit();
	}

	private void _optionsDatasetSelected(TfSpaceData dataset)
	{
		_optionsDataset = dataset;
		_options.SpaceDataId = dataset is null ? null : dataset.Id;
		_generatedColumnsListInit();
	}

	private void _optionsSpaceViewSelected(TfSpaceView view)
	{
		_optionsExistingSpaceView = view;
		_options.SpaceViewId = view.Id;
	}

	private void _columnGeneratorSettingChanged(bool value, string field)
	{

		if (field == nameof(_options.AddProviderColumns))
		{
			_options.AddProviderColumns = value;
		}
		else if (field == nameof(_options.AddSharedColumns))
		{
			_options.AddSharedColumns = value;
		}
		else if (field == nameof(_options.AddSystemColumns))
		{
			_options.AddSystemColumns = value;
		}
		else if (field == nameof(_options.AddDataSetColumns))
		{
			_options.AddDataSetColumns = value;
		}
		_generatedColumnsListInit();
	}

	private void _generatedColumnsListInit()
	{
		_generatedColumns.Clear();

		if (_optionsDataProvider is not null)
		{
			if (_options.AddProviderColumns)
				_generatedColumns.AddRange(_optionsDataProvider.Columns.Select(x => x.DbName));
			if (_options.AddSystemColumns)
				_generatedColumns.AddRange(_optionsDataProvider.SystemColumns.Select(x => x.DbName));
			if (_options.AddSharedColumns)
				_generatedColumns.AddRange(_optionsDataProvider.SharedColumns.Select(x => x.DbName));
		}
		else if (_optionsDataset is not null)
		{
			if (_options.AddDataSetColumns)
			{
				if (_optionsDataset.Columns.Count > 0)
				{
					_generatedColumns.AddRange(_optionsDataset.Columns.Select(x => x));
				}
				else if (_allDataProviders.Any(x => x.Id == _optionsDataset.DataProviderId))
				{
					var dataProvider = _allDataProviders.Single(x => x.Id == _optionsDataset.DataProviderId);
					_generatedColumns.AddRange(dataProvider.Columns.Select(x => x.DbName));
				}
			}
		}
	}

	private async Task _goToView()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if (navState.SpaceId is null || _optionsExistingSpaceView is null || _optionsExistingSpaceView.Id == Guid.Empty) return;

		Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, navState.SpaceId, _optionsExistingSpaceView.Id));
	}

	private async Task _onNodeEdit() => await Context.EditNode.InvokeAsync();
	private async Task _onNodeDelete() => await Context.DeleteNode.InvokeAsync();

	#endregion
}

public class TfSpaceViewSpacePageAddonOptions
{
	[JsonPropertyName("SetType")]
	public TfSpaceViewSetType SetType { get; set; } = TfSpaceViewSetType.New;

	[JsonPropertyName("SpaceViewId")]
	public Guid? SpaceViewId { get; set; } = null;

	[JsonPropertyName("Name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("Type")]
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;

	[JsonPropertyName("DataSetType")]
	public TfSpaceViewDataSetType DataSetType { get; set; } = TfSpaceViewDataSetType.New;

	[JsonPropertyName("DataProviderId")]
	public Guid? DataProviderId { get; set; } = null;

	[JsonPropertyName("NewSpaceDataName")]
	public string NewSpaceDataName { get; set; } = null;

	[JsonPropertyName("SpaceDataId")]
	public Guid? SpaceDataId { get; set; } = null;

	[JsonPropertyName("AddSystemColumns")]
	public bool AddSystemColumns { get; set; } = false;
	[JsonPropertyName("AddProviderColumns")]
	public bool AddProviderColumns { get; set; } = true;
	[JsonPropertyName("AddSharedColumns")]
	public bool AddSharedColumns { get; set; } = true;
	[JsonPropertyName("AddDatasetColumns")]
	public bool AddDataSetColumns { get; set; } = true;

}