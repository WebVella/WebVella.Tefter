using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.Email.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.Email.Components.Settings.SettingsComponent", "WebVella.Tefter.TemplateProcessors.Email")]
public partial class SettingsComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorSettingsComponentContext>
{
	[Inject] public ITfTemplateService TemplateService { get; set; }
	[Inject] public ITfBlobManager BlobManager { get; set; }

	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorSettingsComponentContext Context { get; set; }

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
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		contentProcessor = new EmailTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(Context.Template.SettingsJson);
		Context.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<EmailTemplateSettings>(Context.Template.SettingsJson);
			base.InitForm(_form);
		}
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_templatesAll = contentProcessor.GetTemplateSelectionList(Context.Template.Id, TemplateService);
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
		Context.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await Context.SettingsJsonChanged.InvokeAsync(Context.Template.SettingsJson);
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