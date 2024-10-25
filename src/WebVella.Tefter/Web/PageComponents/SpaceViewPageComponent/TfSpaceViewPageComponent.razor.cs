namespace WebVella.Tefter.Web.PageComponents;
[LocalizationResource("WebVella.Tefter.Web.PageComponents.SpaceViewPageComponent.TfSpaceViewPageComponent", "WebVella.Tefter")]
public partial class TfSpaceViewPageComponent : TucBaseSpaceNodeComponent
{
	#region << Render Injects >>
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	#endregion

	#region << Base Overrides >>
	public override Guid Id { get; set; } = new Guid("68afeecc-6ca9-4102-831d-ef4028057128");
	public override string Name { get; set; } = "SpaceView";
	public override string Description { get; set; } = "present data in a grid format";
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
			if (_options.Id is null)
			{
				ValidationErrors.Add(new ValidationError(nameof(_options.Id), "required"));
			}
		}

		return ValidationErrors;
	}

	public override async Task OnNodeCreated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context)
	{
		await base.OnNodeCreated(serviceProvider, context);
		throw new NotImplementedException();
	}
	public override async Task OnNodeUpdated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context)
	{
		await base.OnNodeUpdated(serviceProvider, context);
		throw new NotImplementedException();
	}
	public override async Task OnNodeDeleted(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context)
	{
		await base.OnNodeDeleted(serviceProvider, context);
		throw new NotImplementedException();
	}
	#endregion

	#region << Private properties >>
	private string optionsJson = "{}";
	private TfSpaceViewPageComponentSettings _options { get; set; } = new();
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
			_options = JsonSerializer.Deserialize<TfSpaceViewPageComponentSettings>(optionsJson);
		}
	}
	#endregion

	#region << Private methods >>
	private void _optionsSetTypeChangeHandler(TucSpaceViewSetType type)
	{
		_options.SetType = type;
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

	private void _optionsDataProviderSelectedHandler(string providerIdString)
	{
		_optionsDataProvider = null;
		_options.DataProviderId = null;
		Guid providerId = Guid.Empty;
		if (!String.IsNullOrWhiteSpace(providerIdString) && Guid.TryParse(providerIdString, out providerId))
			if (providerId == Guid.Empty) return;

		var provider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == providerId);
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
		_options.Id = view.Id;
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

public class TfSpaceViewPageComponentSettings
{
	[JsonIgnore]
	public TucSpaceViewSetType SetType { get; set; } = TucSpaceViewSetType.New;

	[JsonPropertyName("Id")]
	public Guid? Id { get; set; } = null;

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