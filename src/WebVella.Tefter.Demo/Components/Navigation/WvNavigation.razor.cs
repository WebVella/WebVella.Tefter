namespace WebVella.Tefter.Demo.Components;
public partial class WvNavigation : WvBaseComponent
{
	private Space selectedSpace = new();
	private List<MenuItem> spaceItems = new();
	private bool spaceSelectorVisible = false;
	private bool spaceSettingsVisible = false;
	protected override void OnInitialized()
	{
		var spaces = WvState.GetSpaces();
		selectedSpace = spaces[0];
		foreach (var place in selectedSpace.Items)
		{
			spaceItems.Add(new MenuItem
			{
				Id = place.Id,
				Icon = place.Icon,
				Match = NavLinkMatch.Prefix,
				Title = place.Name,
				Url = $"/space/{place.Id}"
			});
		}

		
	}
}