namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewSettingsSelector.TfSpaceViewSettingsSelector", "WebVella.Tefter")]
public partial class TfSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
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
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.Id));
	}
	private void _manageData()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceView.SpaceDataId));
	}

	private async Task _deleteView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this view deleted?")))
			return;
		try
		{

			UC.DeleteSpaceView(TfAppState.Value.SpaceView.Id);
			ToastService.ShowSuccess(LOC("Space view deleted"));
			if (TfAppState.Value.SpaceViewList.Count > 0)
				Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceViewList[0].Id), true);
			else
				Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id), true);
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