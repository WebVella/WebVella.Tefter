namespace WebVella.Tefter.TemplateProcessors.FileGroup.Addons;

public partial class DisplaySettingsAddon : TfBaseComponent,
	ITfScreenRegionAddon<TfTemplateProcessorDisplaySettingsScreenRegion>
{
	[Inject] public ITfService TfService { get; set; } = null!;
	public const string ID = "96699142-2CAF-433A-971A-D1C20AB5DEA9";
	public const string NAME = "FileGroup Template View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(FileGroupTemplateProcessor),null)
	};
	[Parameter] 
	public TfTemplateProcessorDisplaySettingsScreenRegion RegionContext { get; set; }

	private bool _loading = true;
	private string _activeTab = SettingsComponentTabs.Attachments.ToDescriptionString();
	private FileGroupTemplateSettings _form = new();
	private List<TfTemplate> _templatesAll = new();
	private List<FileGroupTemplateSettingsAttachmentItemDisplay> _attachmentsSelection = new();
	private FileGroupTemplateProcessor contentProcessor { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		contentProcessor = new FileGroupTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<FileGroupTemplateSettings>(RegionContext.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<FileGroupTemplateSettings>(RegionContext.Template.SettingsJson);
			_recalcAttachmentData();
		}
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_templatesAll = contentProcessor.GetTemplateSelectionList(RegionContext.Template.Id, TfService);
			_recalcAttachmentData();
			_loading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _recalcAttachmentData()
	{
		_attachmentsSelection = new();
		foreach (var item in _form.AttachmentItems)
		{
			var attachment = _templatesAll.Where(x => x.Id == item.TemplateId).FirstOrDefault();
			if (attachment is null) continue;
			_attachmentsSelection.Add(new FileGroupTemplateSettingsAttachmentItemDisplay
			{
				TemplateId = item.TemplateId,
				Template = attachment,
			});
		}
	}


}