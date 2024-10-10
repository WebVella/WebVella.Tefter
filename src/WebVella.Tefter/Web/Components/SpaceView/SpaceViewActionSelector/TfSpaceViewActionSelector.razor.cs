namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _open = false;

	public void ToggleSelector()
	{
		_open = !_open;
	}

	private Task _deleteSelectedRecords()
	{
		if (TfAppState.Value.SelectedDataRows.Count == 0
		|| TfAppState.Value.SpaceView.SpaceDataId is null) return Task.CompletedTask;
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
		return Task.CompletedTask;
	}
}