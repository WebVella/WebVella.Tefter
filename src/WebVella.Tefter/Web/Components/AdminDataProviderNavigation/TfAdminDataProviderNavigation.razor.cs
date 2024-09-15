namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.AdminDataProviderNavigation.TfAdminDataProviderNavigation","WebVella.Tefter")]
public partial class TfAdminDataProviderNavigation : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }


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
		await UC.Init(this.GetType());
		UC.MenuLoading = false;
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_DataProviderDetailsChangedAction);
	}

	private void loadMoreClick()
	{
		UC.LoadMoreLoading = true;
		StateHasChanged();
		UC.MenuPage++;
		UC.InitMenu();
		UC.LoadMoreLoading = false;
		StateHasChanged();
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
			ToastService.ShowSuccess("Data provider successfully created!");
			Dispatcher.Dispatch(new SetDataProviderAdminAction(false, provider));
			Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, provider.Id));
		}
	}


	private void onSearch(string search)
	{
		if (search == UC.MenuSearch) return;

		UC.MenuLoading = true;
		StateHasChanged();

		UC.MenuSearch = search;
		UC.OnSearchChanged();

		UC.MenuLoading = false;
		StateHasChanged();
	}

	private void On_DataProviderDetailsChangedAction(DataProviderAdminChangedAction action)
	{
		UC.OnStateChanged(action.Provider);
		StateHasChanged();
	}
}