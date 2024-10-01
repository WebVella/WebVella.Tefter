namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _open = false;
	private bool _selectorLoading = false;

	private List<TucScreenRegionComponentMeta> _regionComponents = new();
	private long _lastRegionRenderedTimestamp = 0;


	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1000); //load components with actions?
			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _deleteSelectedRecords()
	{
		if (TfAppState.Value.SelectedDataRows.Count == 0
		|| TfAppState.Value.SpaceView.SpaceDataId is null) return;
		try
		{
			var result = UC.DeleteSpaceDataRows(
				spaceDataId:TfAppState.Value.SpaceView.SpaceDataId.Value,
				tfIdList:TfAppState.Value.SelectedDataRows
			);
			ProcessServiceResponse(result);
			if(result.IsSuccess)
				Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}