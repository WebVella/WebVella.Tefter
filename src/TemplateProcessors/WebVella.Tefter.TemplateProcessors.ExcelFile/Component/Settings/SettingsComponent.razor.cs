using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Settings.SettingsComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class SettingsComponent : TfFormBaseComponent, ITfCustomComponent
{
	[Inject] public ITfBlobManager BlobManager { get; set; }

	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;

	[Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
	public string Value
#pragma warning restore BL0007 // Component parameters should be auto properties
	{
		get => JsonSerializer.Serialize(_form);
		set
		{
			if (String.IsNullOrEmpty(value))
			{
				_form = new();
			}
			else
			{
				_form = JsonSerializer.Deserialize<ExcelFileTemplateSettings>(value);
			}
			if(_form is null) _form = new();
		}
	}

	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private ExcelFileTemplateSettings _form = new();
	private string _downloadUrl { get { 
		if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
		{
			return String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
		}	
		return null;
	} }
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
	}


	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();


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
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_form));
		_fileLoading = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _groupByChanged(List<string> input)
	{
		_form.GroupBy = input;
		await ValueChanged.InvokeAsync(JsonSerializer.Serialize(_form));
	}

}