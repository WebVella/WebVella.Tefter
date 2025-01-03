using Microsoft.AspNetCore.Components.Forms;

namespace WebVella.Tefter.TemplateProcessors.ExcelFile.Components;

[LocalizationResource("WebVella.Tefter.TemplateProcessors.ExcelFile.Components.Help.HelpComponent", "WebVella.Tefter.TemplateProcessors.ExcelFile")]
public partial class HelpComponent : TfFormBaseComponent, ITfCustomComponent
{
	//For this component only ReadOnly and Form will be supported
	[Parameter] public TfComponentMode DisplayMode { get; set; } = TfComponentMode.Read;
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public object Context { get; set; }

	private ExcelFileTemplateSettings _form = new();
	private string _fileLocalPath = null;
	private string _downloadUrl = null;
	private bool _fileLoading = false;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		base.InitForm(_form);
		_initFileSettings();
	}

	private void _initFileSettings()
	{
		_downloadUrl = null;
		if (_form.TemplateFileBlobId is not null && _form.TemplateFileBlobId != Guid.Empty)
		{
			_downloadUrl = String.Format(TfConstants.BlobDownloadUrl, _form.TemplateFileBlobId, _form.FileName);
		}
	}

	public List<ValidationError> Validate()
	{
		MessageStore.Clear();
		var errors = new List<ValidationError>();


		var isValid = EditContext.Validate();
		StateHasChanged();
		return errors;
	}
	private async Task _fileChanged(Tuple<string,string> input)
	{
		_fileLoading = true;
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1000);
		_form.FileName = input.Item2;
		_form.TemplateFileBlobId = Guid.NewGuid();


		_fileLoading = true;
		await InvokeAsync(StateHasChanged);
	}

}