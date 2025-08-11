namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewSettingsSelector.TucSpaceViewSettingsSelector", "WebVella.Tefter")]
public partial class TucSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	private bool _open = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}

	private void _manageView()
	{
		Navigator.NavigateTo(string.Format(TfConstants.SpaceViewPageUrl, SpaceView.SpaceId, SpaceView.Id));
	}
	private void _manageData()
	{
		Navigator.NavigateTo(string.Format(TfConstants.SpaceDataPageUrl, SpaceView.SpaceId, SpaceView.SpaceDataId));
	}

	private async Task _deleteView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this view deleted?")))
			return;
		try
		{

			TfSpaceViewUIService.DeleteSpaceView(SpaceView.Id);
			ToastService.ShowSuccess(LOC("Space view deleted"));

			var spacePages = TfSpaceUIService.GetSpacePages(SpaceView.SpaceId);
			Guid? firstPageId = null;
			foreach (var page in spacePages)
			{
				var pageId = page.GetFirstNavigatedPageId();
				if (pageId is not null)
				{
					firstPageId = pageId;
					break;
				}
			}
			if (firstPageId is not null)
				Navigator.NavigateTo(string.Format(TfConstants.SpacePagePageUrl,SpaceView.SpaceId, firstPageId.Value));
			else
				Navigator.NavigateTo(TfConstants.HomePageUrl);
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