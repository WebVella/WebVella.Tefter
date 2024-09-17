namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderNavigation.TfAdminDataProviderNavigation","WebVella.Tefter")]
public partial class TfAdminDataProviderNavigation : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _endIsReached = false;
	private bool _isLoading = false;
	private string _search = null;
	private int _stringLimit = 30;
	private int _pageSize = TfConstants.PageSize;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.AdminDataProviders.Count < _pageSize)
			_endIsReached = true;
	}

	private async Task loadMoreClick(bool resetList = false)
	{
		var page = TfAppState.Value.AdminDataProvidersPage + 1;
		if (resetList)
		{
			page = 1;
			_endIsReached = false;
		}
		_isLoading = true;
		await InvokeAsync(StateHasChanged);

		var records = await UC.GetDataProvidersAsync(
			search: _search,
			page: page,
			pageSize: _pageSize
		);
		var aggrRecords = TfAppState.Value.AdminDataProviders;

		if (!resetList)
		{
			aggrRecords.AddRange(records);
		}
		else
		{
			aggrRecords = records;
		}

		Dispatcher.Dispatch(new SetAppStateAction(
			component: this,
			state: TfAppState.Value with {AdminDataProviders = aggrRecords, AdminDataProvidersPage = page }
		));

		_isLoading = false;
		if (records.Count < _pageSize) _endIsReached = true;
		await InvokeAsync(StateHasChanged);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(new TucDataProvider(), new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var provider = (TucDataProvider)result.Data;
			Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, provider.Id));
		}
	}

	private async Task onSearch(string search)
	{
		_search = search;
		await loadMoreClick(true);
	}
}