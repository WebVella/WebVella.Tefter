namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataManage.TfSpaceDataManage", "WebVella.Tefter")]
public partial class TfSpaceDataManage : TfFormBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	public TucDataProvider SelectedProvider = null;

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private TucSpaceData _form = new();
	private List<TucDataProvider> _joinedProviders = new();
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
		await _init();
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
		InvokeAsync(async () =>
		{
			await _init();
			await InvokeAsync(StateHasChanged);
		});
	}

	private async Task _init()
	{
		if (TfAppState.Value.SpaceData is null) return;
		_form = TfAppState.Value.SpaceData with { Id = TfAppState.Value.SpaceData.Id };
		base.InitForm(_form);
		if (_form.DataProviderId != Guid.Empty)
		{
			SelectedProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _form.DataProviderId);
			_joinedProviders = await UC.GetDataProviderJoinedProvidersAsync(SelectedProvider.Id);
		}
		else
		{
			SelectedProvider = null;
		}
	}

	private async Task _deleteSpaceData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this dataset deleted?")))
			return;
		try
		{
			UC.DeleteSpaceData(TfAppState.Value.SpaceData.Id);

			ToastService.ShowSuccess(LOC("Space dataset deleted"));

			if (TfAppState.Value.SpaceDataList.Count > 0)
				Navigator.NavigateTo(string.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, TfAppState.Value.SpaceDataList[0].Id), true);
			else
				Navigator.NavigateTo(string.Format(TfConstants.SpacePageUrl, TfAppState.Value.Space.Id), true);
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
	private async Task _editSpaceData()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		TfAppState.Value.SpaceData,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceData)result.Data;
			var itemList = TfAppState.Value.SpaceDataList.ToList();
			var itemIndex = itemList.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				itemList[itemIndex] = item;
			}

			ToastService.ShowSuccess(LOC("Space dataset successfully updated!"));

			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = item,
				SpaceDataList = itemList
			}));
		}
	}
	private async Task _onColumnsChanged(List<string> columns)
	{
		try
		{
			TucSpaceData spaceData = UC.UpdateSpaceDataColumns(TfAppState.Value.SpaceData.Id, columns);
			ToastService.ShowSuccess("Dataset updated!");
			var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
			var itemIndex = spaceDataList.FindIndex(x => x.Id == spaceData.Id);
			if (itemIndex > -1) spaceDataList[itemIndex] = spaceData;
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = spaceData,
				SpaceDataList = spaceDataList
			}));
			_form = spaceData with { Id = spaceData.Id };
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
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
			TucSpaceData submitResult = UC.UpdateSpaceDataFilters(TfAppState.Value.SpaceData.Id, _form.Filters);
			ToastService.ShowSuccess("Dataset updated!");
			var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
			var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Id);
			if (itemIndex > -1) spaceDataList[itemIndex] = submitResult;
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = submitResult,
				SpaceDataList = spaceDataList
			}));
			_form = submitResult with { Id = submitResult.Id };
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
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
			TucSpaceData submitResult = UC.UpdateSpaceDataSorts(TfAppState.Value.SpaceData.Id, sorts);
			ToastService.ShowSuccess("Dataset updated!");
			var spaceDataList = TfAppState.Value.SpaceDataList.ToList();
			var itemIndex = spaceDataList.FindIndex(x => x.Id == submitResult.Id);
			if (itemIndex > -1) spaceDataList[itemIndex] = submitResult;
			Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with
			{
				SpaceData = submitResult,
				SpaceDataList = spaceDataList
			}));
			_form = submitResult with { Id = submitResult.Id };
		}
		catch (Exception ex)
		{
			ProcessFormSubmitResponse(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}
}

