namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataManage.TfSpaceDataManage", "WebVella.Tefter")]
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	public TucDataProvider SelectedProvider = null;

	private string _error = string.Empty;
	private string _activeTab = "columns";
	private bool _isSubmitting = false;
	private TucSpaceData _form = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_init();
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		_init();
		StateHasChanged();
	}

	private void _init()
	{
		_form = TfAppState.Value.SpaceData with { Id = TfAppState.Value.SpaceData.Id };
		base.InitForm(_form);
		if (_form.DataProviderId != Guid.Empty)
		{
			SelectedProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _form.DataProviderId);
		}
		else
		{
			SelectedProvider = null;
		}
	}

	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			Result<TucSpaceData> submitResult = UC.UpdateSpaceDataColumns(TfAppState.Value.SpaceData.Id, columns);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceData = submitResult.Value,
					SpaceDataList = spaceDataList
				}));
				_form = submitResult.Value with { Id = submitResult.Value.Id };
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

	private async Task _onFiltersChanged(List<TucFilterBase> filters)
	{
		_form.Filters = filters;
		await _saveFilters();
	}

	private async Task _saveFilters()
	{
		if (_isSubmitting) return;
		try
		{
			Result<TucSpaceData> submitResult = UC.UpdateSpaceDataFilters(TfAppState.Value.SpaceData.Id, _form.Filters);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceData = submitResult.Value,
					SpaceDataList = spaceDataList
				}));
				_form = submitResult.Value with { Id = submitResult.Value.Id };
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _onSortChanged(List<TucSort> sorts)
	{
		try
		{
			Result<TucSpaceData> submitResult = UC.UpdateSpaceDataSorts(TfAppState.Value.SpaceData.Id, sorts);
			ProcessFormSubmitResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				ToastService.ShowSuccess("Dataset updated!");
				var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
				var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Value.Id);
				if (itemIndex > -1) spaceDataList[itemIndex] = submitResult.Value;
				Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: TfAppState.Value with
				{
					SpaceData = submitResult.Value,
					SpaceDataList = spaceDataList
				}));
				_form = submitResult.Value with { Id = submitResult.Value.Id };
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
