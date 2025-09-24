using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.Email.Addons;

public partial class ManageSettingsComponent : TfFormBaseComponent, 
	ITfScreenRegionComponent<TfTemplateProcessorManageSettingsScreenRegionContext>
{
	[Inject] public ITfService TfService { get; set; } = default!;
	public const string ID = "0a15ce68-12af-4fae-a464-10bf5c0a1c9b";
	public const string NAME = "Email Template Settings Manage";
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
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegionContext RegionContext { get; set; }

	private bool _loading = true;
	private string _activeTab = SettingsComponentTabs.Content.ToDescriptionString();
	private EmailTemplateSettings _form = new();
	private List<TfTemplate> _templatesAll = new();
	private List<TfTemplate> _templatesOptions = new();
	private TfTemplate _selectedOption = null;
	private List<EmailTemplateSettingsAttachmentItemDisplay> _attachmentsSelection = new();
	private EmailTemplateProcessor contentProcessor { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		contentProcessor = new EmailTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(RegionContext.Template.SettingsJson);
		RegionContext.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(RegionContext.Template.SettingsJson);
			_recalcAttachmentData();
			base.InitForm(_form);
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
		_templatesOptions = _templatesAll.ToList();
		foreach (var item in _form.AttachmentItems)
		{
			var attachment = _templatesAll.Where(x => x.Id == item.TemplateId).FirstOrDefault();
			if (attachment is null) continue;
			_attachmentsSelection.Add(new EmailTemplateSettingsAttachmentItemDisplay
			{
				TemplateId = item.TemplateId,
				Template = attachment,
			});
			_templatesOptions = _templatesOptions.Where(x => x.Id != item.TemplateId).ToList();
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.Sender))
			errors.Add(new ValidationError(nameof(EmailTemplateSettings.Sender), LOC("required")));

		if (String.IsNullOrWhiteSpace(_form.Recipients))
			errors.Add(new ValidationError(nameof(EmailTemplateSettings.Recipients), LOC("required")));

		if (String.IsNullOrWhiteSpace(_form.Subject))
			errors.Add(new ValidationError(nameof(EmailTemplateSettings.Subject), LOC("required")));

		if (String.IsNullOrWhiteSpace(_form.HtmlContent))
			errors.Add(new ValidationError(nameof(EmailTemplateSettings.HtmlContent), LOC("required")));

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}

		if (errors.Count > 0)
			_activeTab = SettingsComponentTabs.Content.ToDescriptionString();
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}

	private async Task _valueChanged()
	{
		_form.TextContent = null; //to be regenerated from service
		RegionContext.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.Template.SettingsJson);
	}

	private async Task _selectedOptionChanged(TfTemplate option)
	{
		if (option is null) return;
		var templateId = option.Id;
		//_attachmentsSelection = null;
		bool isSelected = _form.AttachmentItems.Any(x => x.TemplateId == templateId);
		if (isSelected) return;
		_form.AttachmentItems.Add(new EmailTemplateSettingsAttachmentItem
		{
			TemplateId = templateId
		});
		_recalcAttachmentData();
		await _valueChanged();
	}
	private async Task _removeItem(EmailTemplateSettingsAttachmentItemDisplay item)
	{
		var index = _form.AttachmentItems.FindIndex(x => x.TemplateId == item.TemplateId);
		if (index == -1) return;
		_form.AttachmentItems.RemoveAt(index);
		_recalcAttachmentData();
		await _valueChanged();
	}
}

public enum SettingsComponentTabs
{
	[Description("content")]
	Content,
	[Description("attachments")]
	Attachments,
	[Description("groupby")]
	GroupBy
}