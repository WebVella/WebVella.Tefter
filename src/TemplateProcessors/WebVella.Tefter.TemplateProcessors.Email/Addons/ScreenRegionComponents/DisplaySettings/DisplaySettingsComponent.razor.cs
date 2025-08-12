namespace WebVella.Tefter.TemplateProcessors.Email.Addons;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Addons.ScreenRegionComponents.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class DisplaySettingsComponent : TfBaseComponent,
	ITfScreenRegionComponent<TfTemplateProcessorDisplaySettingsScreenRegionContext>
{
	[Inject] public ITfService TfService { get; set; } = default!;
	public const string ID = "a52465b2-6e8c-48b3-a903-7fb2d43c55fa";
	public const string NAME = "Email Template View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;

	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] 
	public TfTemplateProcessorDisplaySettingsScreenRegionContext RegionContext { get; set; }

	private bool _loading = true;
	private string _activeTab = SettingsComponentTabs.Content.ToDescriptionString();
	private EmailTemplateSettings _form = new();
	private List<TfTemplate> _templatesAll = new();
	private List<EmailTemplateSettingsAttachmentItemDisplay> _attachmentsSelection = new();
	private EmailTemplateProcessor contentProcessor { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		contentProcessor = new EmailTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(RegionContext.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(RegionContext.Template.SettingsJson);
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
			_attachmentsSelection.Add(new EmailTemplateSettingsAttachmentItemDisplay
			{
				TemplateId = item.TemplateId,
				Template = attachment,
			});
		}
	}


}