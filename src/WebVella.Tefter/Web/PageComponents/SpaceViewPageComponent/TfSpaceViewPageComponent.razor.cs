namespace WebVella.Tefter.Web.PageComponents;
[LocalizationResource("WebVella.Tefter.Web.PageComponents.SpaceViewPageComponent.TfSpaceViewPageComponent", "WebVella.Tefter")]
public partial class TfSpaceViewPageComponent : TucBaseSpaceNodeComponent
{
	#region << Render Injects >>
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	#endregion

	#region << Base Overrides >>
	public override Guid Id { get; set; } = new Guid("68afeecc-6ca9-4102-831d-ef4028057128");
	public override string Name { get; set; } = "Space View";
	public override string Description { get; set; } = "present data in a grid format";
	public override string FluentIconName { get; set; } = "Table";
	[Parameter] public override TfSpaceNodeComponentContext Context { get; set; }

	public override string GetOptions() => JsonSerializer.Serialize(_options);
	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();
		if (_options.SetType == TucSpaceViewSetType.New)
		{
			if (String.IsNullOrWhiteSpace(_options.Name))
			{
				ValidationErrors.Add(new ValidationError(nameof(_options.Name), "required"));
			}
			if (_options.DataSetType == TucSpaceViewDataSetType.New)
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

	public override Task<(TfAppState, TfAuxDataState)> InitState(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState,
		TfSpaceNodeComponentContext context)
	{
		#region << Init Options >>
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) return Task.FromResult((newAppState, newAuxDataState)); ;

		var options = JsonSerializer.Deserialize<TfSpaceViewPageComponentOptions>(context.ComponentOptionsJson);
		if (options is null || options.SpaceViewId is null) return Task.FromResult((newAppState, newAuxDataState)); ;
		#endregion

		#region << Init SpaceView>>
		newAppState = newAppState with { Route = newAppState.Route with {SpaceViewId = options.SpaceViewId }};
		#endregion

	
		return Task.FromResult((newAppState, newAuxDataState));
	}

	public override async Task<string> OnNodeCreated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context)
	{
		await base.OnNodeCreated(serviceProvider, context);
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson)) throw new Exception("TfSpaceViewPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<TfSpaceViewPageComponentOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("TfSpaceViewPageComponent error: options cannot be deserialized");
		Guid? originalSpaceViewId = jsonOptions.SpaceViewId;
		var spaceman = serviceProvider.GetService<ITfSpaceManager>();
		if (jsonOptions.SetType == TucSpaceViewSetType.New)
		{
			//Create view if needed
			var spaceView = new TucSpaceView
			{
				Id = Guid.NewGuid(),
				AddDatasetColumns = jsonOptions.AddDatasetColumns,
				AddProviderColumns = jsonOptions.AddProviderColumns,
				SpaceDataId = jsonOptions.SpaceDataId,
				AddSharedColumns = jsonOptions.AddSharedColumns,
				AddSystemColumns = jsonOptions.AddSystemColumns,
				DataProviderId = jsonOptions.DataProviderId,
				DataSetType = jsonOptions.DataSetType,
				Name = jsonOptions.Name,
				NewSpaceDataName = jsonOptions.NewSpaceDataName,
				Position = 1,
				Presets = new List<TucSpaceViewPreset>(),
				Settings = new TucSpaceViewSettings(),
				SpaceId = context.SpaceId,
				Type = jsonOptions.Type,
			};

			var serviceResult = spaceman.CreateSpaceView(spaceView.ToModelExtended(), spaceView.DataSetType == TucSpaceViewDataSetType.New);
			if (serviceResult.IsFailed) throw new Exception("TfSpaceViewPageComponent error: CreateSpaceView failed");

			jsonOptions.SpaceViewId = serviceResult.Value.Id;
			jsonOptions.SpaceDataId = serviceResult.Value.SpaceDataId;
			context.ComponentOptionsJson = JsonSerializer.Serialize(jsonOptions);
		}
		return context.ComponentOptionsJson;
	}

	#endregion

	#region << Private properties >>
	private string optionsJson = "{}";
	private TfSpaceViewPageComponentOptions _options { get; set; } = new();
	private TucDataProvider _optionsDataProvider = null;
	private TucSpaceData _optionsDataset = null;
	private TucSpaceView _optionsExistingSpaceView = null;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 10;
	#endregion


	#region << Render Lifecycle >>
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.ComponentOptionsJson != optionsJson)
		{
			optionsJson = Context.ComponentOptionsJson;
			_options = JsonSerializer.Deserialize<TfSpaceViewPageComponentOptions>(optionsJson);
			//When cannot node has json from another page type
			if(_options is null) _options = new TfSpaceViewPageComponentOptions();

			if (_options.SpaceViewId is not null)
				_optionsExistingSpaceView = TfAppState.Value.SpaceViewList.FirstOrDefault(x => x.Id == _options.SpaceViewId);
			if (_optionsExistingSpaceView is not null)
				_optionsDataset = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == _optionsExistingSpaceView.SpaceDataId);
			if (_options.DataProviderId is not null)
				_optionsDataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _options.DataProviderId);
			_generatedColumnsListInit();
		}
	}
	#endregion

	#region << Private methods >>
	private void _optionsSetTypeChangeHandler(TucSpaceViewSetType type)
	{
		_options.SetType = type;
		if (type == TucSpaceViewSetType.Existing)
		{
			if (_optionsExistingSpaceView is null && TfAppState.Value.SpaceViewList.Any())
			{
				_optionsExistingSpaceView = TfAppState.Value.SpaceViewList[0];
				_options.SpaceViewId = _optionsExistingSpaceView.Id;
			}
		}
	}
	private void _optionsDataSetTypeChangeHandler(TucSpaceViewDataSetType type)
	{
		_optionsDataProvider = null;
		_options.DataProviderId = null;
		_optionsDataset = null;
		_options.SpaceDataId = null;
		_options.DataSetType = type;

		if (type == TucSpaceViewDataSetType.New)
		{
			if (TfAppState.Value.AllDataProviders.Any())
			{
				_options.DataProviderId = TfAppState.Value.AllDataProviders[0].Id;
				_optionsDataProvider = TfAppState.Value.AllDataProviders[0];
			}
		}
		else if (type == TucSpaceViewDataSetType.Existing)
		{
			if (TfAppState.Value.SpaceDataList.Any())
			{
				_options.SpaceDataId = TfAppState.Value.SpaceDataList[0].Id;
				_optionsDataset = TfAppState.Value.SpaceDataList[0];
			}
		}
		_generatedColumnsListInit();
	}

	private void _optionsDataProviderSelectedHandler(TucDataProvider provider)
	{
		_optionsDataProvider = null;
		_options.DataProviderId = null;

		if (provider is null) return;
		_optionsDataProvider = provider;
		_options.DataProviderId = provider.Id;
		_generatedColumnsListInit();
	}

	private void _optionsDatasetSelected(TucSpaceData dataset)
	{
		_optionsDataset = dataset;
		_options.SpaceDataId = dataset is null ? null : dataset.Id;
		_generatedColumnsListInit();
	}

	private void _optionsSpaceViewSelected(TucSpaceView view)
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
		else if (field == nameof(_options.AddDatasetColumns))
		{
			_options.AddDatasetColumns = value;
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
			if (_options.AddDatasetColumns)
				_generatedColumns.AddRange(_optionsDataset.Columns.Select(x => x));
		}
	}

	#endregion
}

public class TfSpaceViewPageComponentOptions
{
	[JsonPropertyName("SetType")]
	public TucSpaceViewSetType SetType { get; set; } = TucSpaceViewSetType.New;

	[JsonPropertyName("SpaceViewId")]
	public Guid? SpaceViewId { get; set; } = null;

	[JsonPropertyName("Name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("Type")]
	public TucSpaceViewType Type { get; set; } = TucSpaceViewType.DataGrid;

	[JsonPropertyName("DataSetType")]
	public TucSpaceViewDataSetType DataSetType { get; set; } = TucSpaceViewDataSetType.New;

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
	public bool AddDatasetColumns { get; set; } = true;

}