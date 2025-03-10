namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.DisplaySettings.DisplaySettingsComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class DisplaySettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfTemplateProcessorDisplaySettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("a52465b2-6e8c-48b3-a903-7fb2d43c55fa");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Email Template View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(EmailTemplateProcessor),null)
	};
	[Parameter] 
	public TfTemplateProcessorDisplaySettingsComponentContext Context { get; init; }

	private bool _loading = true;
	private string _activeTab = SettingsComponentTabs.Content.ToDescriptionString();
	private EmailTemplateSettings _form = new();
	private List<TfTemplate> _templatesAll = new();
	private List<EmailTemplateSettingsAttachmentItemDisplay> _attachmentsSelection = new();
	private EmailTemplateProcessor contentProcessor { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		contentProcessor = new EmailTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(Context.Template.SettingsJson);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(Context.Template.SettingsJson);
			_recalcAttachmentData();
		}
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_templatesAll = contentProcessor.GetTemplateSelectionList(Context.Template.Id, TfService);
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