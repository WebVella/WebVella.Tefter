namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewShareSelector : TfBaseComponent
{
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	[Parameter] public TfBookmark? ActiveBookmark { get; set; } = null;
	[Parameter] public TfBookmark? ActiveSavedUrl { get; set; } = null;
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
			RouteState = TfState.NavigationState
		});
	}

	private string? _getExportAllData()
	{
		if (SpaceView is null) return null;
		return JsonSerializer.Serialize(new TfExportViewData()
		{
			SelectedRows = new(),
			RouteState = TfState.NavigationState
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


	private async Task _bookmarkView()
	{
		try
		{
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpacePageId = SpaceView.Id,
				UserId = TfState.User!.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = null
			};

			TfService.CreateBookmark(submit);
			ToastService.ShowSuccess(LOC("View is now bookmarked"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _saveViewUrl()
	{
		try
		{
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpacePageId = SpaceView.Id,
				UserId = TfState.User.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			TfService.CreateBookmark(submit);

			ToastService.ShowSuccess(LOC("URL is now saved"));
			await Navigator.ApplyChangeToUrlQuery(TfConstants.ActiveSaveQueryName, submit.Id);

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}
}