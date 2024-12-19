﻿namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.FileField.TfFileField", "WebVella.Tefter")]
public partial class TfFileField : TfBaseComponent
{
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public bool ReadOnly { get; set; } = false;
	[Parameter] public string Value { get; set; }
	[Parameter] public string FileName { get; set; }
	[Parameter] public string DownloadUrl { get; set; }
	[Parameter] public EventCallback<Tuple<string,string>> ValueChanged { get; set; }
	[Parameter] public EventCallback ResetRequested { get; set; }

	private string _originalValue = null;
	private string _fileName
	{
		get
		{
			if(!String.IsNullOrWhiteSpace(FileName)) return FileName;
			if(!String.IsNullOrWhiteSpace(Value)) return Path.GetFileName(Value);

			return null;
		}
	}
	private FluentInputFileEventArgs _upload = null;
	private string _uploadId = $"tf-{Guid.NewGuid()}";
	FluentInputFile? fileUploader = default!;
	int progressPercent = 0;
	List<FluentInputFileEventArgs> Files = new();


	protected override void OnInitialized()
	{
		base.OnInitialized();
		_originalValue = Value;
	}

	private async Task _clearFile()
	{
		await ValueChanged.InvokeAsync(null);
	}

	private async Task _resetFile()
	{
		await ResetRequested.InvokeAsync();
	}

	private async Task _onCompleted(IEnumerable<FluentInputFileEventArgs> files)
	{
		Files = files.ToList();
		progressPercent = fileUploader!.ProgressPercent;

		if (Files.Count > 0)
		{
			_upload = Files[0];
			if (_upload is not null)
			{
				await ValueChanged.InvokeAsync(new Tuple<string,string>(_upload.LocalFile.ToString(),_upload.Name));
				//_form.LocalPath = _upload.LocalFile.ToString();
				//_form.FileName = _upload.Name;
				//_getNameFromPath(false);
			}
		}
	}

	private void _onProgress(FluentInputFileEventArgs e)
	{
		progressPercent = e.ProgressPercent;
	}
}
