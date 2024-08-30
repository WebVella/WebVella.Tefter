﻿using WebVella.Tefter.Web.Components.SpaceManageDialog;
using WebVella.Tefter.Web.Components.SpaceSelector;
using WebVella.Tefter.Web.Components.SpaceDataManageDialog;

namespace WebVella.Tefter.Web.Components.SpaceDataNavigation;
public partial class TfSpaceDataNavigation : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IState<UserState> UserState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }

	private bool _menuLoading = false;


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
		foreach (var dataItem in SpaceState.Value.SpaceDataList)
		{
			if (!String.IsNullOrWhiteSpace(search) && !dataItem.Name.ToLowerInvariant().Contains(search))
				continue;

			var menuItem = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(dataItem.Id),
				Icon = TfConstants.SpaceDataIcon,
				Match = NavLinkMatch.Prefix,
				Level = 0,
				Title = dataItem.Name,
				Url = String.Format(TfConstants.SpaceDataPageUrl, dataItem.SpaceId, dataItem.Id),
				SpaceId = dataItem.SpaceId,
				SpaceDataId = dataItem.Id,
				Active = dataItem.Id == SpaceState.Value.RouteSpaceDataId,
				Expanded = false
			};
			SetMenuItemActions(menuItem);
			_menuItems.Add(menuItem);

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
		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		new TucSpaceData{ SpaceId = SpaceState.Value.Space.Id},
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceData)result.Data;
			ToastService.ShowSuccess(LOC("Space dataset successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl,item.SpaceId, item.Id));
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
			if (item.SpaceId is null || item.SpaceDataId is null) return;
			Navigator.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, item.SpaceId, item.SpaceDataId));
		}
	}

	private void onViewsListClick(){ 
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, SpaceState.Value.Space.Id, null));
	}

	private async Task onSearch(string value)
	{
		search = value;
		GenerateMenu(search);
		await InvokeAsync(StateHasChanged);
	}
}