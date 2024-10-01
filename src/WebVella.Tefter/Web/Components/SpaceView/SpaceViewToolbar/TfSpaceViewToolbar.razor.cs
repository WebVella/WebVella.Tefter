namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewToolbar.TfSpaceViewToolbar", "WebVella.Tefter")]
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public EventCallback<string> OnSearch { get; set; }

	private async Task _searchChanged(string value) => await OnSearch.InvokeAsync(value);

	private Task _onAddRowClick()
	{
		if (TfAppState.Value.SpaceViewData is null) return Task.CompletedTask;
		try
		{
			var newDt = TfAppState.Value.SpaceViewData.NewTable();
			newDt.Rows.Add(newDt.NewRow());

			var result = UC.SaveDataDataTable(newDt);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				var clone = TfAppState.Value.SpaceViewData.Clone();
				clone.Rows.Insert(0,result.Value.Rows[0]);
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { SpaceViewData = clone }));

				ToastService.ShowSuccess(LOC("Row added"));
			}

		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}
}