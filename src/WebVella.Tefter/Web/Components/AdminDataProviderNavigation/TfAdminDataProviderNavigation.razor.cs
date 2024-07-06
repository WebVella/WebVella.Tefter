using WebVella.Tefter.Web.Components.DataProviderManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderNavigation;
public partial class TfAdminDataProviderNavigation : TfBaseComponent
{
	[Inject] private DataProviderAdminUseCase UC { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }


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
		_setMenuItemActions();
		UC.MenuLoading = false;
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		ActionSubscriber.SubscribeToAction<DataProviderAdminChangedAction>(this, On_DataProviderDetailsChangedAction);
	}

	private void _setMenuItemActions()
	{
		foreach (var item in UC.MenuItems)
		{
			item.OnClick = () => OnTreeMenuClick(item);
		}
	}

	private void loadMoreClick()
	{
		UC.LoadMoreLoading = true;
		StateHasChanged();
		UC.MenuPage++;
		UC.InitMenu();
		_setMenuItemActions();
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

	private void OnTreeMenuClick(TucMenuItem item)
	{
		if (item.Data is null) return;
		item.Active = true;
		Navigator.NavigateTo(item.Url);
	}

	private void onSearch(string search)
	{
		if (search == UC.MenuSearch) return;

		UC.MenuLoading = true;
		StateHasChanged();

		UC.MenuSearch = search;
		UC.OnSearchChanged();
		_setMenuItemActions();

		UC.MenuLoading = false;
		StateHasChanged();
	}

	private void On_DataProviderDetailsChangedAction(DataProviderAdminChangedAction action)
	{
		UC.OnStateChanged(action.Provider);
		_setMenuItemActions();
		StateHasChanged();
	}
}