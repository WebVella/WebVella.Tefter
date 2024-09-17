namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewNavigation.TfSpaceViewNavigation", "WebVella.Tefter")]
public partial class TfSpaceViewNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private bool _menuLoading = false;

	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;


	private string search = null;
	private bool hasMore = true;
	private int page = 1;
	private int pageSize = 30;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}
	private void On_StateChanged(SpaceStateChangedAction action)
	{
		InvokeAsync(async () =>
		{
			_menuLoading = true;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
			GenerateMenu();
			_menuLoading = false;
			await InvokeAsync(StateHasChanged);
		});

	}

	private void GenerateMenu(string search = null)
	{
		search = search?.Trim().ToLowerInvariant();
		_menuItems.Clear();
		var nodes = new List<MenuItem>();
		foreach (var view in TfAppState.Value.SpaceViewList)
		{
			if (!String.IsNullOrWhiteSpace(search) && !view.Name.ToLowerInvariant().Contains(search))
				continue;

			var viewMenu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
				Icon = TfConstants.SpaceViewIcon,
				Match = NavLinkMatch.Prefix,
				Title = view.Name,
				Url = String.Format(TfConstants.SpaceViewPageUrl, view.SpaceId, view.Id),
			};
			_menuItems.Add(viewMenu);
		}

		var batch = _menuItems.Skip(RenderUtils.CalcSkip(page, pageSize)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems = batch;
	}

	private async Task loadMoreClick()
	{
		var batch = _menuItems.Skip(RenderUtils.CalcSkip(page + 1, pageSize)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems.AddRange(batch);
		page++;
		await InvokeAsync(StateHasChanged);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		new TucSpaceView() with { SpaceId = TfRouteState.Value.SpaceId.Value },
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceView)result.Data;
			ToastService.ShowSuccess(LOC("Space view successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, item.SpaceId, item.Id));
		}
	}

	private async Task onManageSpaceClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceManageDialog>(
		TfAppState.Value.Space,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpace)result.Data;
			ToastService.ShowSuccess(LOC("Space successfully updated!"));
			//Change user state > spaces
			var userSpaces = TfAppState.Value.CurrentUserSpaces.ToList();
			var itemIndex = userSpaces.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				userSpaces[itemIndex] = item;
			}
			var state = TfAppState.Value with { CurrentUserSpaces = userSpaces };
			if (TfAppState.Value.Space is not null
				&& TfAppState.Value.Space.Id == item.Id)
			{
				state = state with { Space = item };
			}

			Dispatcher.Dispatch(new SetAppStateAction(
				component: this,
				state: state
			));
		}
	}

	private void onDataListClick()
	{
		Guid? spaceDataId = null;
		if (TfAppState.Value.SpaceDataList.Count > 0) spaceDataId = TfAppState.Value.SpaceDataList[0].Id;
		Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, TfAppState.Value.Space.Id, spaceDataId));
	}
	private async Task onSearch(string value)
	{
		search = value;
		GenerateMenu(search);
		await InvokeAsync(StateHasChanged);
	}
}