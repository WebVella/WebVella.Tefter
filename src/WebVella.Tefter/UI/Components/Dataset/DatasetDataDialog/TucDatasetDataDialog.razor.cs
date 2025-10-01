using WebVella.Tefter.Models;

namespace WebVella.Tefter.UI.Components;

public partial class TucDatasetDataDialog : TfBaseComponent, IDialogContentComponent<TfDataset?>
{
	[Parameter] public TfDataset? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private string _error = string.Empty;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn = null!;
	private bool _isCreate = false;
	private TfDataTable? _data = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Current Data");
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			await _init();
	}

	private async Task _init()
	{

		try
		{
			_error = string.Empty;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);

			_data = TfUIService.QueryDataset(
				datasetId: Content!.Id,
				page: 1,
				pageSize: TfConstants.ItemsMaxLimit
			);
		}
		catch (Exception ex)
		{
			_error = ex.Message;
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}


	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}
}
