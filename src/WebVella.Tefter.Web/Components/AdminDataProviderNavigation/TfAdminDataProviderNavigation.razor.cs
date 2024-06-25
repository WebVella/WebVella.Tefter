﻿using WebVella.Tefter.Web.Components.DataProviderManageDialog;

namespace WebVella.Tefter.Web.Components.AdminDataProviderNavigation;
public partial class TfAdminDataProviderNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }


	private bool _menuLoading = true;
	private List<MenuItem> _menuItems = new();
	private List<MenuItem> _visibleMenuItems = new();

	private string search = null;
	private bool hasMore = true;
	private int page = 1;
	private int pageSize = TfConstants.PageSize;

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
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateSpaceDataMenu();
			ActionSubscriber.SubscribeToAction<DataProviderDetailsChangedAction>(this, On_DataProviderDetailsChangedAction);
			_menuLoading = false;
			StateHasChanged();
		}
	}

	private void GenerateSpaceDataMenu(string search = null)
	{
		search = search?.Trim().ToLowerInvariant();
		_menuItems.Clear();
		var userResult = DataProviderManager.GetProviders();
		if (userResult.IsFailed) return;
		var users = userResult.Value.OrderBy(x => x.Name).ToList();
		foreach (var item in users)
		{
			if (!String.IsNullOrWhiteSpace(search) && !item.Name.ToLowerInvariant().Contains(search))
				continue;
			var menu = new MenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Data = item,
				Icon = new Icons.Regular.Size20.Connector(),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = item.Name,
				Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, item.Id),
				Active = Navigator.GetUrlData().DataProviderId == item.Id,

			};
			SetMenuItemActions(menu);
			_menuItems.Add(menu);

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
		var dialog = await DialogService.ShowDialogAsync<TfDataProviderManageDialog>(null, new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (User)result.Data;
			ToastService.ShowSuccess("Data provider successfully created!");
			Dispatcher.Dispatch(new SetUserDetailsAction(user));
			Navigator.NavigateTo(String.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}

	private void SetMenuItemActions(MenuItem item)
	{
		item.OnSelect = (selected) => OnTreeMenuSelect(item, selected);
	}

	private void OnTreeMenuSelect(MenuItem item, bool selected)
	{
		item.Active = selected;
		if (item.Active && item.Data is not null)
		{
			var user = (User)item.Data;
			Navigator.NavigateTo(String.Format(TfConstants.AdminDataProviderDetailsPageUrl, user.Id));
		}
	}

	private async Task onSearch(string value)
	{
		search = value;
		GenerateSpaceDataMenu(search);
		await InvokeAsync(StateHasChanged);
	}

	private void On_DataProviderDetailsChangedAction(DataProviderDetailsChangedAction action)
	{
		InvokeAsync(async () =>
		{
			GenerateSpaceDataMenu(search);
			await InvokeAsync(StateHasChanged);
		});
	}
}