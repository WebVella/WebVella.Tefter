namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataProviderData.TfAdminDataProviderData", "WebVella.Tefter")]
public partial class TfAdminDataProviderData : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _isListBusy = false;
	private bool _showSystemColumns = false;
	private bool _showSharedKeyColumns = false;
	private bool _showCustomColumns = true;
	private string _search = null;

	private void _toggleSystemColumns()
	{
		_showSystemColumns = !_showSystemColumns;
		StateHasChanged();
	}
	private void _toggleSharedKeyColumns()
	{
		_showSharedKeyColumns = !_showSharedKeyColumns;
		StateHasChanged();
	}
	private void _toggleCustomColumns()
	{
		_showCustomColumns = !_showCustomColumns;
		StateHasChanged();
	}

	private async Task _deleteAllData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need all data deleted? This operation can take minutes!")))
			return;
		await UC.DeleteAllProviderData(TfAppState.Value.AdminDataProvider.Id);
		ToastService.ShowSuccess(LOC("Data provider data deletion is triggered!"));
		Navigator.ReloadCurrentUrl();
	}

	private async Task _searchChanged(string? search)
	{
		_search = search;
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var page = 1;
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _goFirstPage()
	{
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var page = 1;
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _goPreviousPage()
	{
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var page = TfAppState.Value.AdminDataProviderDataPage - 1;
			if (page <= 0) page = 1;
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _goNextPage()
	{
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var page = TfAppState.Value.AdminDataProviderDataPage + 1;
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _goLastPage()
	{
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var page = -1;
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _goOnPage(int page)
	{
		_isListBusy = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var result = UC.GetDataProviderDataResult(
				providerId: TfAppState.Value.AdminDataProvider.Id,
				search: _search,
				page: page,
				pageSize: TfConstants.PageSize
			);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				Dispatcher.Dispatch(new SetAppStateAction(
					component: this,
					state: TfAppState.Value with
					{
						AdminDataProviderData = result.Value,
						AdminDataProviderDataPage = page
					}
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}