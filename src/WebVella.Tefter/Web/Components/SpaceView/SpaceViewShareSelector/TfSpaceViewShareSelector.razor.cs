namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewShareSelector.TfSpaceViewShareSelector", "WebVella.Tefter")]
public partial class TfSpaceViewShareSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	private bool _open = false;
	private string _exportExcelUrl = "/api/export/export-view";
	private string _exportSelectionBtnId = "tfExportSelectionBtn";
	private string _exportAllBtnId = "tfExportAllBtn";
	public void ToggleSelector()
	{
		_open = !_open;
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