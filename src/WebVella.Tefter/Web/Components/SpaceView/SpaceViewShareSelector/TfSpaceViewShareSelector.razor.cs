namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewShareSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	private bool _open = false;
	private bool _selectorLoading = false;
	private string _exportExcelUrl = "/api/export/export-view";
	private string _exportSelectionBtnId = "tfExportSelectionBtn";
	private string _exportAllBtnId = "tfExportAllBtn";
	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1000);//loading components?

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _copyLinkToClipboard()
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", Navigator.Uri);
		ToastService.ShowSuccess(LOC("Link copied"));
	}

	private string _getExportSelectionData()
	{
		if (TfAppState.Value.SpaceView is null) return null;
		return JsonSerializer.Serialize(new TucExportViewData()
		{
			SelectedRows = TfAppState.Value.SelectedDataRows,
			RouteState = TfRouteState.Value
		});
	}

	private string _getExportAllData()
	{
		if (TfAppState.Value.SpaceView is null) return null;
		return JsonSerializer.Serialize(new TucExportViewData()
		{
			SelectedRows = new(),
			RouteState = TfRouteState.Value
		});
	}


	private async Task _exportSelection()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElement", _exportSelectionBtnId);
	}
	private async Task _exportAll()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElement", _exportAllBtnId);
	}

}