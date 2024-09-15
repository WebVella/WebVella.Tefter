namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{

	internal async Task InitForNavigationAsync()
	{
		MenuLoading = false;
		await InitMenuAsync();
	}

	internal async Task InitMenuAsync()
	{
		if (MenuPage == 1)
		{
			MenuItems.Clear();
			MenuHasMore = true;
		}
		var userResult = await _identityManager.GetUsersAsync();
		if (userResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetUsersAsync failed").CausedBy(userResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return;
		}
		if (userResult.Value is null) return;

		var search = MenuSearch?.Trim().ToLowerInvariant();
		var users = userResult.Value
			.Where(x => String.IsNullOrWhiteSpace(search)
				|| x.FirstName.ToLowerInvariant().Contains(search)
				|| x.LastName.ToLowerInvariant().Contains(search))
			.Skip(RenderUtils.CalcSkip(MenuPageSize, MenuPage))
			.Take(MenuPageSize).ToList();

		var menuPathSuffix = "";
		var urlData = _navigationManager.GetUrlData();
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
		{
			menuPathSuffix = $"/{urlData.SegmentsByIndexDict[3]}";
		}
		var userIcon = TfConstants.GetIcon("Person");
		foreach (var item in users)
		{
			var menu = new TucMenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Data = new TucUser(item),
				Icon = userIcon,
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = String.Join(" ", new List<string> { item.FirstName, item.LastName }),
				Url = String.Format(TfConstants.AdminUserDetailsPageUrl, item.Id) + menuPathSuffix,
				Active = urlData.UserId == item.Id,

			};
			MenuItems.Add(menu);
		}
		MenuItems = MenuItems.OrderBy(x => x.Title).ToList();
		if (users.Count < MenuPageSize) MenuHasMore = false;

	}
	internal void NavigationOnStateChanged(TucUser user)
	{
		if (user == null) return;
		var urlData = _navigationManager.GetUrlData();
		var userIndex = MenuItems.FindIndex(x => ((TucUser)x.Data).Id == user.Id);

		if (userIndex > -1)
		{
			MenuItems[userIndex] = MenuItems[userIndex] with
			{
				Data = user,
				Title = String.Join(" ", new List<string> { user.FirstName, user.LastName })
			};
		}
		else
		{
			var menu = new TucMenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(user.Id),
				Data = user,
				Icon = TfConstants.GetIcon("Person"),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = String.Join(" ", new List<string> { user.FirstName, user.LastName })
			};
			MenuItems.Add(menu);
		}

		MenuItems = MenuItems.OrderBy(x => x.Title).ToList();
		var menuPathSuffix = "";
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
		{
			menuPathSuffix = $"/{urlData.SegmentsByIndexDict[3]}";
		}
		foreach (var menu in MenuItems)
		{
			var tucUser = (TucUser)menu.Data;
			menu.Active = urlData.UserId == tucUser.Id;
			menu.Url = String.Format(TfConstants.AdminUserDetailsPageUrl, tucUser.Id) + menuPathSuffix;
		}
	}
	internal async Task NavigationOnSearchChanged()
	{
		MenuPage = 1;
		await InitMenuAsync();
	}
}
