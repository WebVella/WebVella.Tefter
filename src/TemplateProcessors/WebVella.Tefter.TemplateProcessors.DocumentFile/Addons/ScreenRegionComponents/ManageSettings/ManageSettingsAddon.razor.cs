using Microsoft.AspNetCore.Components.Forms;
using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.TemplateProcessors.DocumentFile.Addons;

public partial class ManageSettingsAddon : TfFormBaseComponent, 
	ITfScreenRegionAddon<TfTemplateProcessorManageSettingsScreenRegion>
{
	[Inject] public ITfService TfService { get; set; } = null!;
	public const string ID = "d3c8a6d1-02b0-4611-81ae-ac1581fe93a3";
	public const string NAME = "Document Template Manage Seettings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(DocumentFileTemplateProcessor),null)
	};
	[Parameter] public TfTemplateProcessorManageSettingsScreenRegion? RegionContext { get; set; }

	private DocumentFileTemplateSettings _form = new();
	private string _downloadUrl
	{
		get
		{
			if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
			{
				return String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
			}
			return string.Empty;
		}
	}
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (RegionContext is null || RegionContext.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
		RegionContext.Validate = _validate;
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext!.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<DocumentFileTemplateSettings>(RegionContext.Template.SettingsJson) ?? new();
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.FileName))
			errors.Add(new ValidationError(nameof(DocumentFileTemplateSettings.FileName), LOC("required")));

		foreach (var item in errors)
		{
			MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
		}

		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
	private async Task _fileChanged(Tuple<string, string> input)
	{
		_fileLoading = true;
		await InvokeAsync(StateHasChanged);

		if (input is not null)
		{
			var blobId = TfService.CreateBlob(input.Item1, true);
			_form.TemplateFileBlobId = blobId;
			_form.FileName = input.Item2;
		}
		else
		{
			_form.FileName = null;
			_form.TemplateFileBlobId = null;
		}
		await _valueChanged();
		_fileLoading = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _valueChanged()
	{
		RegionContext!.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.Template.SettingsJson);
	}

}