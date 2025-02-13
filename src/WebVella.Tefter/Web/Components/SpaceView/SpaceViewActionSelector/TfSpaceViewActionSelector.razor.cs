namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewActionSelector : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	private bool _open = false;

	public async Task ToggleSelector()
	{
		_open = !_open;
		await InvokeAsync(StateHasChanged);
	}

	private Task _deleteSelectedRecords()
	{
		if (TfAppState.Value.SelectedDataRows.Count == 0
		|| TfAppState.Value.SpaceView.SpaceDataId is null) return Task.CompletedTask;
		try
		{
			UC.DeleteSpaceDataRows(
				spaceDataId: TfAppState.Value.SpaceView.SpaceDataId.Value,
				tfIdList: TfAppState.Value.SelectedDataRows
			);
			Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}
}