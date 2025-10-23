namespace WebVella.Tefter.UI.Components;

public partial class TucDragDrop : TfBaseComponent
{
	[Parameter] public EventCallback<List<FluentInputFileEventArgs>> OnFileDroped { get; set; }

	private int _progressPercent = 0;

	private string _elementId;

	protected override void OnInitialized()
	{
		_elementId = $"tf-file-uploader-{ComponentId}";
	}

	private async Task OnCompletedAsync(IEnumerable<FluentInputFileEventArgs>? files)
	{
		try
		{
			if (files is null) return;
			await OnFileDroped.InvokeAsync(files.ToList());
		}
		finally
		{
			// Wait 3 seconds before to reset the progress bar.
			await Task.Delay(3000);
			_progressPercent = 0;
		}
	}
}