namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal bool MenuLoading { get; set; } = true;
	internal bool LoadMoreLoading { get; set; } = false;
	internal List<TucMenuItem> MenuItems { get; set; } = new();
	internal string MenuSearch { get; set; }
	internal bool MenuHasMore { get; set; } = true;
	internal int MenuPage { get; set; } = 1;
	internal int MenuPageSize { get; set; } = TfConstants.PageSize;

	internal Task InitForNavigation()
	{
		MenuLoading = false;
		InitMenu();
		return Task.CompletedTask;
	}

	internal void InitMenu()
	{
		if (MenuPage == 1)
		{
			MenuItems.Clear();
			MenuHasMore = true;
		}
		var providerResult = _dataProviderManager.GetProviders();
		if (providerResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(providerResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return;
		}
		if (providerResult.Value is null) return;

		var search = MenuSearch?.Trim().ToLowerInvariant();
		var providers = providerResult.Value
			.Where(x => String.IsNullOrWhiteSpace(search)
				|| x.Name.ToLowerInvariant().Contains(search))
			.Skip(RenderUtils.CalcSkip(MenuPageSize, MenuPage))
			.Take(MenuPageSize).ToList();

		var menuPathSuffix = "";
		var urlData = _navigationManager.GetUrlData();
		if (urlData.SegmentsByIndexDict.ContainsKey(3))
		{
			menuPathSuffix = $"/{urlData.SegmentsByIndexDict[3]}";
		}

		foreach (var item in providers)
		{
			var menu = new TucMenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Data = new TucDataProvider(item),
				Icon = new Icons.Regular.Size20.Connector(),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = item.Name,
				Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, item.Id) + menuPathSuffix,
				Active = urlData.DataProviderId == item.Id,

			};
			MenuItems.Add(menu);
		}
		MenuItems = MenuItems.OrderBy(x => x.Title).ToList();
		if (providers.Count < MenuPageSize) MenuHasMore = false;

	}
	internal void OnStateChanged(TucDataProvider provider)
	{
		if (provider == null) return;
		var urlData = _navigationManager.GetUrlData();
		var userIndex = MenuItems.FindIndex(x => ((TucDataProvider)x.Data).Id == provider.Id);

		if (userIndex > -1)
		{
			MenuItems[userIndex] = MenuItems[userIndex] with
			{
				Data = provider,
				Title = provider.Name
			};
		}
		else
		{
			var menu = new TucMenuItem
			{
				Id = RenderUtils.ConvertGuidToHtmlElementId(provider.Id),
				Data = provider,
				Icon = new Icons.Regular.Size20.Connector(),
				Level = 0,
				Match = NavLinkMatch.Prefix,
				Title = provider.Name
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
			var tucProvider = (TucDataProvider)menu.Data;
			menu.Active = urlData.DataProviderId == tucProvider.Id;
			menu.Url = String.Format(TfConstants.AdminDataProviderDetailsPageUrl, tucProvider.Id) + menuPathSuffix;
		}
	}

	internal void OnSearchChanged()
	{
		MenuPage = 1;
		InitMenu();
	}

}
