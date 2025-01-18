using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.TextFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.TextFile.Components.Settings.SettingsComponent", "WebVella.Tefter.TemplateProcessors.TextFile")]
public partial class SettingsComponent : TfFormBaseComponent, ITfDynamicComponent<TfTemplateProcessorSettingsComponentContext>
{
	[Inject] public ITfBlobManager BlobManager { get; set; }

	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public TfTemplateProcessorSettingsComponentContext Context { get; set; }
	private TextFileTemplateSettings _form = new();
	private string _downloadUrl
	{
		get
		{
			if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
			{
				return String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
			}
			return null;
		}
	}
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (Context is null || Context.Template is null) throw new Exception("Context is not defined");
		_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(Context.Template.SettingsJson);
		Context.Validate = _validate;
		base.InitForm(_form);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.Template.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.Template.SettingsJson) ? new() : JsonSerializer.Deserialize<TextFileTemplateSettings>(Context.Template.SettingsJson);
			base.InitForm(_form);
		}
	}

	private List<ValidationError> _validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();
		if (String.IsNullOrWhiteSpace(_form.FileName))
			errors.Add(new ValidationError(nameof(TextFileTemplateSettings.FileName), LOC("required")));

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
			var blobResult = BlobManager.CreateBlob(input.Item1, true);
			ProcessServiceResponse(blobResult);
			if (blobResult.IsSuccess)
			{
				_form.TemplateFileBlobId = blobResult.Value;
				_form.FileName = input.Item2;
			}
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
		Context.Template.SettingsJson = JsonSerializer.Serialize(_form);
		await Context.SettingsJsonChanged.InvokeAsync(Context.Template.SettingsJson);
	}
}