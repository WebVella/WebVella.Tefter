namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;
public partial record TfAppState
{
	public TucSpace Space { get; init; }
	public TfColor SpaceColor
	{
		get
		{
			if (Space is null) return TfColor.Black;

			return Space.Color;
		}
	}
	//Navigation
	public List<TucSpace> CurrentUserSpaces { get; init; } = new();
	public List<TucSpaceNode> SpaceNodes { get; init; } = new();
	public List<TucMenuItem> SpacesNav
	{
		get
		{
			var result = new List<TucMenuItem>();
			if (CurrentUserSpaces is null || CurrentUserSpaces.Count == 0) return result;
			foreach (var item in CurrentUserSpaces.OrderBy(x => x.Position).Take(TfConstants.NavSpacesMax))
			{
				result.Add(new TucMenuItem
				{
					IconCollapsed = item.Icon,
					IconExpanded = item.Icon,
					Id = TfConverters.ConvertGuidToHtmlElementId(item.Id),
					Match = NavLinkMatch.Prefix,
					Url = item.Url,
					Text = item.Name,
					IconColor = item.Color
				});
			}
			return result;
		}
	}
	public TucSpaceNode SpaceNode { get; init; } = null;
	public object SpaceNodeData { get; init; } = null;
}
