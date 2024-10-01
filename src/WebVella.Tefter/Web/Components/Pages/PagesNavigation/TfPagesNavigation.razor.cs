namespace WebVella.Tefter.Web.Components;
public partial class TfPagesNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private string search = null;

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
				Icon = TfConstants.ApplicationIcon,
				Match = NavLinkMatch.Prefix,
				Title = page.Name,
				Url = String.Format(TfConstants.PagesSinglePageUrl, page.Slug),
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