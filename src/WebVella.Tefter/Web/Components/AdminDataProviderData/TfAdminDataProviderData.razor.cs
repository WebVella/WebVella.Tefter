namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderData.TfAdminDataProviderData","WebVella.Tefter")]
public partial class TfAdminDataProviderData : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IState<DataProviderAdminState> DataProviderDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_GetDataProviderDetailsActionResult);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			await UC.LoadDataProviderDataTable(providerId: DataProviderDetailsState.Value.Provider.Id);
			UC.IsBusy = false;
			UC.IsListBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void On_GetDataProviderDetailsActionResult(DataProviderAdminChangedAction action)
	{
		if (action.Provider is null) return;
		base.InvokeAsync(async () =>
		{
			UC.IsBusy = true;
			await InvokeAsync(StateHasChanged);
			await UC.LoadDataProviderDataTable(providerId: DataProviderDetailsState.Value.Provider.Id);
			UC.IsBusy = false;
			UC.IsListBusy = false;
			await InvokeAsync(StateHasChanged);
		});

	}



	private void _toggleSystemColumns()
	{
		UC.ShowSystemColumns = !UC.ShowSystemColumns;
		StateHasChanged();
	}
	private void _toggleSharedKeyColumns()
	{
		UC.ShowSharedKeyColumns = !UC.ShowSharedKeyColumns;
		StateHasChanged();
	}
	private void _toggleCustomColumns()
	{
		UC.ShowCustomColumns = !UC.ShowCustomColumns;
		StateHasChanged();
	}

	private async Task _searchChanged(string? search)
	{
		UC.Search = search;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		await UC.LoadDataProviderDataTable(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goFirstPage()
	{
		if (UC.IsListBusy) return;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		await UC.DataProviderDataTableGoFirstPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goPreviousPage()
	{
		if (UC.IsListBusy) return;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		await UC.DataProviderDataTableGoPreviousPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goNextPage()
	{
		if (UC.IsListBusy) return;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		await UC.DataProviderDataTableGoNextPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}
	private async Task _goLastPage()
	{
		if (UC.IsListBusy) return;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		await UC.DataProviderDataTableGoLastPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}

	private async Task _goOnPage(int page)
	{
		if (UC.IsListBusy) return;
		UC.IsListBusy = true;
		await InvokeAsync(StateHasChanged);
		UC.Page = page;
		await UC.DataProviderDataTableGoOnPage(providerId: DataProviderDetailsState.Value.Provider.Id);
		UC.IsListBusy = false;
		await InvokeAsync(StateHasChanged);
	}

}