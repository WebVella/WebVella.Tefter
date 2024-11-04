namespace WebVella.Tefter.Web.Store;
using SystemColor = System.Drawing.Color;
public partial record TfAppState
{
	public TucSpace Space { get; init; }
	public OfficeColor SpaceColor
	{
		get
		{
			if (Space is null) return OfficeColor.Excel;

			return Space.Color;
		}
	}
	public string SpaceColorString
	{
		get
		{
			if (Space is null) return TfConstants.DefaultThemeColor.ToAttributeValue();

			return Space.Color.ToAttributeValue();
		}
	}
	public string SpaceIconColorString
	{
		get => TfConverters.ChangeColorDarknessHex(SpaceColorObject, (float)0.25);
	}
	public SystemColor SpaceColorObject
	{
		get
		{
			if (SpaceColorString == "default")
				return (SystemColor)System.Drawing.ColorTranslator.FromHtml(TfConstants.DefaultThemeColor.ToAttributeValue());
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(SpaceColorString);
		}
	}
	public string SpaceBackgkroundColor => $"{SpaceColorString}25";
	public string SpaceGridBackgkroundColor => $"{SpaceColorString}25";
	public string SpaceGridSelectedColor => $"{SpaceColorString}25";
	public string SpaceBorderColor => $"{SpaceColorString}75";
	public string SpaceBackgroundAccentColor => $"{SpaceColorString}35";
	public string SpaceSidebarStyle => $"background-color:{SpaceBackgkroundColor} !important; border-color:{SpaceBorderColor} !important";

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
