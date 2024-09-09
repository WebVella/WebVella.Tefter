﻿namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewSettingsSelector : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }
	private bool _open = false;
	private bool _selectorLoading = false;

	private void _init()
	{
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _manageView()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewManagePageUrl, SpaceState.Value.Space.Id, SpaceState.Value.SpaceView.Id));
	}
	private void _manageData()
	{
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, SpaceState.Value.Space.Id, SpaceState.Value.SpaceView.SpaceDataId));
	}

	private async Task _deleteView()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this view deleted?")))
			return;
		try
		{

			Result result = UC.DeleteSpaceView(SpaceState.Value.SpaceView.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Space view deleted"));
				Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, SpaceState.Value.Space.Id), true);
			}
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