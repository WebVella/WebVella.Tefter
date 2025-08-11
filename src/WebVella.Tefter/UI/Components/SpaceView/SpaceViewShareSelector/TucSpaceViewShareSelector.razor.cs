namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewShareSelector.TucSpaceViewShareSelector", "WebVella.Tefter")]
public partial class TucSpaceViewShareSelector : TfBaseComponent
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();
	[Parameter] public TfBookmark? ActiveBookmark { get; set; } = null;
	[Parameter] public TfBookmark? ActiveSavedUrl { get; set; } = null;

	private bool _open = false;
	private string _exportExcelUrl = "/api/export/export-view";
	private string _exportSelectionBtnId = "tfExportSelectionBtn";
	private string _exportAllBtnId = "tfExportAllBtn";


	protected override Task OnInitializedAsync()
	{
		return base.OnInitializedAsync();
	}

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
			RouteState = Navigator.GetRouteState()
		});
	}

	private string? _getExportAllData()
	{
		if (SpaceView is null) return null;
		return JsonSerializer.Serialize(new TfExportViewData()
		{
			SelectedRows = new(),
			RouteState = Navigator.GetRouteState()
		});
	}


	private async Task _exportSelection()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportSelectionBtnId);
	}
	private async Task _exportAll()
	{
		await JSRuntime.InvokeAsync<object>("Tefter.clickElementById", _exportAllBtnId);
	}

	private async Task _bookmarkView()
	{
		try
		{
			var currentUser = await TfUserUIService.GetCurrentUserAsync();
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpaceViewId = SpaceView.Id,
				UserId = currentUser!.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = null
			};

			TfUserUIService.CreateBookmark(submit);
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
			var currentUser = await TfUserUIService.GetCurrentUserAsync();
			var submit = new TfBookmark
			{
				Id = Guid.NewGuid(),
				SpaceViewId = SpaceView.Id,
				UserId = currentUser.Id,
				CreatedOn = DateTime.Now,
				Description = String.Empty,//initially nothing is added for convenience
				Name = SpaceView.Name + " " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
				Url = new Uri(Navigator.Uri).PathAndQuery
			};
			TfUserUIService.CreateBookmark(submit);

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