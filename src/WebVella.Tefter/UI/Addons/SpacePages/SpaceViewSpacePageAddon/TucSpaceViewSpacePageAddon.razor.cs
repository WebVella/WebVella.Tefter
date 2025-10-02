namespace WebVella.Tefter.UI.Addons;

public partial class TucSpaceViewSpacePageAddon : TucBaseSpacePageComponent
{
	#region << Base Overrides >>

	private static readonly string Id = "68afeecc-6ca9-4102-831d-ef4028057128";
	public override Guid AddonId { get; init; } = new Guid(Id);
	public override string AddonName { get; init; } = "Data Grid";
	public override string AddonDescription { get; init; } = "present the selected dataset in a grid";
	public override string AddonFluentIconName { get; init; } = "Table";
	[Parameter] public override TfSpacePageAddonContext Context { get; set; } = null!;
	public override string GetOptions() => JsonSerializer.Serialize(_options);

	public override List<ValidationError> ValidateOptions()
	{
		ValidationErrors.Clear();
		if (_options.DatasetId is null)
		{
			ValidationErrors.Add(new ValidationError(nameof(_options.DatasetId), "required"));
		}

		return ValidationErrors;
	}

	public override async Task<string> OnPageCreated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson))
			throw new Exception("TfSpaceViewPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("TfSpaceViewPageComponent error: options cannot be deserialized");
		if (context.Space is null) throw new Exception("TfSpaceViewPageComponent error: Space not provided");
		var tfService = serviceProvider.GetService<ITfService>();
		TfSpaceViewCreateModel spaceView = new();
		if (context.TemplateId is null)
		{
			//Create view if needed
			spaceView = new TfSpaceViewCreateModel
			{
				Id = Guid.NewGuid(),
				DatasetId = jsonOptions.DatasetId,
				Name = context.SpacePage.Name,
				Presets = new List<TfSpaceViewPreset>(),
				Settings = new TfSpaceViewSettings(),
			};
		}
		else
		{
			var templatePage = tfService.GetSpacePage(context.TemplateId.Value);
			if (templatePage is null)
				throw new Exception($"Template page with id: {context.TemplateId.Value} not found");
			if (templatePage.ComponentId != AddonId)
				throw new Exception(
					$"The template page with id: {context.TemplateId.Value} is not from the required type {AddonName}");
			var templateJsonOptions =
				JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(templatePage.ComponentOptionsJson);
			if (templateJsonOptions is null)
				throw new Exception("TfSpaceViewPageComponent error: template page options cannot be deserialized");
			if (templateJsonOptions.SpaceViewId is null)
				throw new Exception(
					$"Template page with id: {context.TemplateId.Value} does not have space view selected");
			var templateView = tfService.GetSpaceView(templateJsonOptions.SpaceViewId.Value);
			if (templateView is null)
				throw new Exception(
					$"Template page with id: {context.TemplateId.Value} has view with id: {templateJsonOptions.SpaceViewId.Value} that does not exist");

			spaceView = new TfSpaceViewCreateModel
			{
				Id = Guid.NewGuid(),
				DatasetId = templateView.DatasetId,
				Name = context.SpacePage.Name,
				Presets = templateView.Presets,
				Settings = templateView.Settings,
			};
		}

		var createdSpaceView = tfService.CreateSpaceView(spaceView);
		jsonOptions.SpaceViewId = createdSpaceView.Id;
		jsonOptions.DatasetId = createdSpaceView.DatasetId;
		context.ComponentOptionsJson = JsonSerializer.Serialize(jsonOptions);
		return context.ComponentOptionsJson;
	}

	public override async Task<string> OnPageUpdated(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		return context.ComponentOptionsJson;
	}

	public override async Task OnPageDeleted(IServiceProvider serviceProvider, TfSpacePageAddonContext context)
	{
		if (String.IsNullOrWhiteSpace(context.ComponentOptionsJson))
			throw new Exception("TfSpaceViewPageComponent error: ComponentOptionsJson is null");
		var jsonOptions = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(context.ComponentOptionsJson);
		if (jsonOptions is null) throw new Exception("TfSpaceViewPageComponent error: options cannot be deserialized");
		var tfService = serviceProvider.GetService<ITfService>();
		if (jsonOptions.SpaceViewId.HasValue)
			tfService.DeleteSpaceView(jsonOptions.SpaceViewId.Value);
	}

	public override List<TfScreenRegionTab> GetManagementTabs()
	{
		var tabs = new List<TfScreenRegionTab>()
		{
			new TfScreenRegionTab("columns", "Columns", "Table"),
			new TfScreenRegionTab("presets", "Preset Filters", "Filter"),
		};
		return tabs;
	}

	#endregion

	#region << Private properties >>

	//Edit
	private TfSpaceViewSpacePageAddonOptions _options { get; set; } = new();
	private TfDataset? _selectedDataset = null;
	private List<string> _generatedColumns = new();
	private int _generatedColumnCountLimit = 10;

	private List<TfDataset> _allDatasets = new();

	#endregion


	#region << Render Lifecycle >>

	protected override void OnInitialized()
	{
		if (Context is null) throw new Exception("Context is required");

		_allDatasets = TfUIService.GetDatasets();

		if (!String.IsNullOrWhiteSpace(Context?.ComponentOptionsJson) && Context!.ComponentOptionsJson != "{}")
			_options = JsonSerializer.Deserialize<TfSpaceViewSpacePageAddonOptions>(Context!.ComponentOptionsJson) ??
			           new();

		if (_options.DatasetId != null && _options.DatasetId != Guid.Empty)
		{
			_selectedDataset = _allDatasets.FirstOrDefault(x => x.Id == _options.DatasetId);
		}
	}

	#endregion

	#region << Private methods >>

	private async Task _valueChanged(string propName, object? payload)
	{
		if (propName == nameof(_options.DatasetId))
		{
			_selectedDataset = null;
			_options.DatasetId = null;
			if (payload is not null)
			{
				_selectedDataset = (TfDataset)payload;
				_options.DatasetId = _selectedDataset.Id;
			}
		}

		await Context.ComponentOptionsJsonChanged.InvokeAsync(JsonSerializer.Serialize(_options));
	}

	#endregion
}

public class TfSpaceViewSpacePageAddonOptions
{
	[JsonPropertyName("DatasetId")] public Guid? DatasetId { get; set; } = null;

	[JsonPropertyName("SpaceViewId")] public Guid? SpaceViewId { get; set; } = null;
}