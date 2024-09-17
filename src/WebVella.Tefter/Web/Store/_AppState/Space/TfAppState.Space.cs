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
		get => RenderUtils.ChangeColorDarknessHex(SpaceColorObject, (float)0.25);
	}
	public SystemColor SpaceColorObject
	{
		get
		{
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(SpaceColorString);
		}
	}
	public string SpaceBackgkroundColor => $"{SpaceColorString}25";
	public string SpaceBorderColor => $"{SpaceColorString}75";
	public string SpaceBackgroundAccentColor => $"{SpaceColorString}35";
	public string SpaceSidebarStyle => $"background-color:{SpaceBackgkroundColor} !important; border-color:{SpaceBorderColor} !important";

	//Navigation
	public List<TucSpace> CurrentUserSpaces { get; init; } = new();
	public List<MenuItem> SpacesNav
	{
		get
		{
			var result = new List<MenuItem>();
			if (CurrentUserSpaces is null || CurrentUserSpaces.Count == 0) return result;
			foreach (var item in CurrentUserSpaces)
			{
				result.Add(new MenuItem
				{
					Icon = item.Icon,
					Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
					Match = NavLinkMatch.Prefix,
					Url = String.Format(TfConstants.SpacePageUrl, item.Id), //item.DefaultViewId - active menu issues
					Title = item.Name,
					IconColor = item.Color,
				});
			}
			return result;
		}
	}
}
