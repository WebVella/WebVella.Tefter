namespace WebVella.Tefter.Web.Store.SessionState;
using System.Drawing;
using SystemColor = System.Drawing.Color;
public partial record SessionState
{
	public bool IsDataLoading { get; init; }
	public string DataHashId { get; init; }
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
	public IDictionary<Guid, SpaceData> SpaceDataDict { get; init; }
	public IDictionary<Guid, SpaceView> SpaceViewDict { get; init; }
	public List<MenuItem> SpaceNav { get; init; } = new();

	public string SpaceColor
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
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(SpaceColor);
		}
	}

	public string SpaceBackgkroundColor => $"{SpaceColor}15";

	public string SpaceBorderColor => $"{SpaceColor}50";

	public string SpaceBackgroundAccentColor => $"{SpaceColor}35";

	public string SpaceSidebarStyle => $"background-color:{SpaceBackgkroundColor} !important; border-color:{SpaceBorderColor} !important";

	public List<Guid> SelectedDataRows { get; init; } = new();
}
