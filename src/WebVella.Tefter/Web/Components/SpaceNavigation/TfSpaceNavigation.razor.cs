using WebVella.Tefter.Web.Components.SpaceManageDialog;
using WebVella.Tefter.Web.Components.SpaceSelector;
using WebVella.Tefter.Web.Components.SpaceViewCreateDialog;

namespace WebVella.Tefter.Web.Components.SpaceNavigation;
public partial class TfSpaceNavigation : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	private bool _menuLoading = false;
	private Guid? _renderedViewId = null;
	private Guid? _renderedSpaceId = null;


	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private bool _selectorMenuVisible = false;
	private bool _settingsMenuVisible = false;
	private TfSpaceSelector _selectorEl;


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
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		GenerateMenu();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}
	private void On_StateChanged(SpaceStateChangedAction action)
	{
		InvokeAsync(async () =>
		{
			if (SpaceState.Value.RouteSpaceViewId == _renderedViewId
				&& SpaceState.Value.RouteSpaceId == _renderedSpaceId) return;

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
		_renderedViewId = SpaceState.Value.RouteSpaceViewId;
		_renderedSpaceId = SpaceState.Value.RouteSpaceId;
		var nodes = new List<MenuItem>();
		foreach (var view in SpaceState.Value.SpaceViewList)
		{
			if (!String.IsNullOrWhiteSpace(search) && !view.Name.ToLowerInvariant().Contains(search))
				continue;

			var viewMenu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(view.Id),
				Icon = TfConstants.SpaceViewIcon,
				Match = NavLinkMatch.Prefix,
				Level = 0,
				Title = view.Name,
				Url = String.Format(TfConstants.SpaceViewPageUrl, view.SpaceId, view.Id),
				SpaceId = view.SpaceId,
				SpaceViewId = view.Id,
				Active = view.Id == _renderedViewId,
				Expanded = false
			};
			SetMenuItemActions(viewMenu);
			_menuItems.Add(viewMenu);

		}

		var batch = _menuItems.Skip(RenderUtils.CalcSkip(pageSize, page)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems = batch;
	}

	private async Task loadMoreClick()
	{
		var batch = _menuItems.Skip(RenderUtils.CalcSkip(pageSize, page + 1)).Take(pageSize).ToList();
		if (batch.Count < pageSize) hasMore = false;
		_visibleMenuItems.AddRange(batch);
		page++;
		await InvokeAsync(StateHasChanged);
	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewCreateDialog>(
		new TucSpaceView() with { SpaceId = SpaceState.Value.RouteSpaceId.Value },
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
		SpaceState.Value.Space,
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
			Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, item.Id));
		}
	}
	private void SetMenuItemActions(MenuItem item)
	{
		item.OnSelect = (selected) => OnTreeMenuSelect(item, selected);
	}

	private void OnTreeMenuSelect(MenuItem item, bool selected)
	{
		item.Active = selected;
		if (item.Active)
		{
			if (item.SpaceId is null || item.SpaceViewId is null) return;
			Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, item.SpaceId, item.SpaceViewId));
		}
	}

	private async Task onSearch(string value)
	{
		search = value;
		GenerateMenu(search);
		await InvokeAsync(StateHasChanged);
	}
}