namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewShareSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	private bool _open = false;
	private string _exportCSVUrl = "/api/export/export-view-to-csv";
	private string _exportExcelUrl = "/api/export/export-view-to-excel";
	private string _exportCSVSelectionBtnId = "tfExportCSVSelectionBtn";
	private string _exportCSVAllBtnId = "tfExportCSVAllBtn";
	private string _exportExcelSelectionBtnId = "tfExportExcelSelectionBtn";
	private string _exportExcelAllBtnId = "tfExportExcelAllBtn";

	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _copyLinkToClipboard()
	{
		await JSRuntime.InvokeVoidAsync("Tefter.copyToClipboard", Navigator.Uri);
		ToastService.ShowSuccess(LOC("Link copied"));
	}

	private string? _getExportSelectionData()
	{
		if (SpaceView is null) return null;
		return JsonSerializer.Serialize(new TfExportViewData()
		{
			SelectedRows = SelectedRows,
			RouteState = TfAuthLayout.GetState().NavigationState
		});
	}

	private string? _getExportAllData()
	{
		if (SpaceView is null) return null;
		return JsonSerializer.Serialize(new TfExportViewData()
		{
			SelectedRows = new(),
			RouteState = TfAuthLayout.GetState().NavigationState
		});
	}


	private async Task _exportCSVSelection()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportCSVSelectionBtnId);
	}
	private async Task _exportCSVAll()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportCSVAllBtnId);
	}

	private async Task _exportExcelSelection()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportExcelSelectionBtnId);
	}
	private async Task _exportExcelAll()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportExcelAllBtnId);
	}

}