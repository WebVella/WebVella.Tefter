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
			var result = UC.DeleteSpaceDataRows(
				spaceDataId: TfAppState.Value.SpaceView.SpaceDataId.Value,
				tfIdList: TfAppState.Value.SelectedDataRows
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
				Navigator.ReloadCurrentUrl();
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}

	private async Task useTemplateWithSelectedRecords()
	{
		if (TfAppState.Value.SelectedDataRows.Count == 0
		|| TfAppState.Value.SpaceView.SpaceDataId is null) return;

		var context = new TucUseTemplateContext
		{
			SelectedRowIds = TfAppState.Value.SelectedDataRows,
			SpaceData = TfAppState.Value.SpaceData,
			User = TfAppState.Value.CurrentUser
		};
		var dialog = await DialogService.ShowDialogAsync<TfUseTemplateDialog>(
				context,
				new DialogParameters()
				{
					PreventDismissOnOverlayClick = true,
					PreventScroll = true,
					Width = TfConstants.DialogWidthExtraLarge,
					TrapFocus = false
				});
	}
}