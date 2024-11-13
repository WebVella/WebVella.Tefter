﻿namespace WebVella.Tefter.Web.Components;
public partial class TfAdminPagesNavigation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

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
			var icon = String.IsNullOrWhiteSpace(page.FluentIconName) ? TfConstants.ApplicationIcon : TfConstants.GetIcon(page.FluentIconName);
			var uri = new Uri(Navigator.Uri);
			var menu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid()),
				IconCollapsed = icon.WithColor(Color.Accent),
				IconExpanded = icon.WithColor(Color.Accent),
				Selected = uri.LocalPath.StartsWith(String.Format(TfConstants.AdminPagesSingleUrl, page.Id)),
				Text = page.Name,
				Url = String.Format(TfConstants.AdminPagesSingleUrl, page.Id),
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