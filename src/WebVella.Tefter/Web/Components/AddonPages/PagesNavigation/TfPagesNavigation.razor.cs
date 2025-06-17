namespace WebVella.Tefter.Web.Components;
public partial class TfPagesNavigation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IStateSelection<TfUserState, bool> SidebarExpanded { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}

	private string search = null;

	private List<TucMenuItem> _getMenu()
	{
		search = search?.Trim().ToLowerInvariant();
		var menuItems = new List<TucMenuItem>();
		var uri = new Uri(Navigator.Uri);
		if (TfAppState.Value.Pages is null) return menuItems;

		foreach (var page in TfAppState.Value.Pages)
		{
			if (!String.IsNullOrWhiteSpace(search) && !page.Name.ToLowerInvariant().Contains(search))
				continue;
			var icon = string.IsNullOrWhiteSpace(page.FluentIconName) ? TfConstants.ApplicationIcon : TfConstants.GetIcon(page.FluentIconName);
			var menu = new TucMenuItem
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid()),
				IconCollapsed = icon.WithColor(Color.Accent),
				IconExpanded = icon.WithColor(Color.Accent),
				Selected = uri.LocalPath.StartsWith(string.Format(TfConstants.PagesSinglePageUrl, page.Id)),
				Text = page.Name,
				Url = string.Format(TfConstants.PagesSinglePageUrl, page.Id),
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