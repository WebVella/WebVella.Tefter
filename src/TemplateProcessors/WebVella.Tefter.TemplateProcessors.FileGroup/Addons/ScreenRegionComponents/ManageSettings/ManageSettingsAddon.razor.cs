using WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;
using WebVella.Tefter.TemplateProcessors.ExcelFile.Addons;
using WebVella.Tefter.TemplateProcessors.TextFile.Addons;

namespace WebVella.Tefter.TemplateProcessors.FileGroup.Addons;

public partial class ManageSettingsAddon : TfFormBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorManageSettingsScreenRegion>
{
	public const string Id = "A79D6CAF-5A72-4B85-9DA7-44244F7B499F";
	public Guid AddonId { get; init; } = new Guid(Id);
	public string AddonName { get; init; } = "FileGroup Template Settings Manage";
	public string AddonDescription { get; init; } = "";
	public string AddonFluentIconName { get; init; } = "PuzzlePiece";
	public int PositionRank { get; init; } = 1000;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(FileGroupTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegion? RegionContext { get; set; }

	private bool _loading = true;
	private string _activeTab = SettingsComponentTabs.Attachments.ToDescriptionString();
	private FileGroupTemplateSettings _form = new();
	private List<TfTemplate> _templatesAll = new();
	private List<TfTemplate> _templatesOptions = new();
	private TfTemplate? _selectedOption = null;
	private List<FileGroupTemplateSettingsAttachmentItemDisplay> _attachmentsSelection = new();
	private FileGroupTemplateProcessor _contentProcessor { get; set; } = null!;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_contentProcessor = new FileGroupTemplateProcessor();
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<FileGroupTemplateSettings>(RegionContext.Template.SettingsJson);
		RegionContext.Validate = _validate;
		base.InitForm(_form);
	}
	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<FileGroupTemplateSettings>(RegionContext.Template.SettingsJson);
			_recalcAttachmentData();
			base.InitForm(_form);
		}
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_templatesAll = _contentProcessor.GetTemplateSelectionList(RegionContext.Template.Id, TfService);
			_recalcAttachmentData();
			_loading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _recalcAttachmentData()
	{
		_attachmentsSelection = new();
		_templatesOptions = _templatesAll.Where(x=>
			x.ContentProcessorType.FullName == typeof(DocumentFileTemplateProcessor).FullName
			|| x.ContentProcessorType.FullName == typeof(ExcelFileTemplateProcessor).FullName
			|| x.ContentProcessorType.FullName == typeof(TextFileTemplateProcessor).FullName
			|| x.ContentProcessorType.FullName == typeof(FileGroupTemplateProcessor).FullName
			).ToList();
		foreach (var item in _form.AttachmentItems)
		{
			var attachment = _templatesAll.Where(x => x.Id == item.TemplateId).FirstOrDefault();
			if (attachment is null) continue;
			_attachmentsSelection.Add(new FileGroupTemplateSettingsAttachmentItemDisplay
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
		//No validation required 

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}

		if (errors.Count > 0)
			_activeTab = SettingsComponentTabs.Attachments.ToDescriptionString();
		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}

	private async Task _valueChanged()
	{
		//_form.TextContent = null; //to be regenerated from service
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
		_form.AttachmentItems.Add(new FileGroupTemplateSettingsAttachmentItem
		{
			TemplateId = templateId
		});
		_recalcAttachmentData();
		await _valueChanged();
	}
	private async Task _removeItem(FileGroupTemplateSettingsAttachmentItemDisplay item)
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
	[Description("attachments")]
	Attachments,
	[Description("groupby")]
	GroupBy
}