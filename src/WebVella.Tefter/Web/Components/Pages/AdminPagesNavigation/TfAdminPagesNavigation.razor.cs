﻿namespace WebVella.Tefter.Web.Components;
public partial class TfAdminPagesNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;
	private int _stringLimit = 30;
	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		foreach (var page in TfAppState.Value.Pages)
		{
			if (!String.IsNullOrWhiteSpace(search) && !page.Name.ToLowerInvariant().Contains(search))
				continue;

			var menu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid()),
				IconCollapsed = TfConstants.ApplicationIcon,
				Match = NavLinkMatch.Prefix,
				Text = page.Name,
				Url = String.Format(TfConstants.AdminPagesSingleUrl, page.Slug),
			};
			menuItems.Add(menu);
		}

		return menuItems;
	}

	private void onSearch(string value)
	{
		search = value;
	}
}