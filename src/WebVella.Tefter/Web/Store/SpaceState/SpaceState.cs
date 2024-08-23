namespace WebVella.Tefter.Web.Store.SpaceState;
using System.Drawing;
using SystemColor = System.Drawing.Color;

[FeatureState]
public partial record SpaceState
{
	public bool IsBusy { get; init; } = true;
	public Guid? RouteSpaceId { get; init; }
	public Guid? RouteSpaceDataId { get; init; }
	public Guid? RouteSpaceViewId { get; init; }
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
			if (RouteSpaceId is null || Space is null) return TfConstants.DefaultThemeColor.ToAttributeValue();

			return Space.Color.ToAttributeValue();
		}
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
}
