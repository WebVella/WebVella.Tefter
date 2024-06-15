namespace WebVella.Tefter.Web.Store.SessionState;
using SystemColor = System.Drawing.Color;

[FeatureState]
public record SessionState
{
	public bool IsLoading { get; init; } = true;
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool SidebarExpanded { get; init; } = true;
	public bool IsDataLoading { get; init; }
	public string DataHashId { get; init; }
	public Guid? SpaceRouteId { get; init; }
	public Guid? SpaceDataRouteId { get; init; }
	public Guid? SpaceViewRouteId { get; init; }
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
	public IEnumerable<Space> SpaceList { get; init; }
	public IDictionary<Guid, Space> SpaceDict { get; init; }
	public IEnumerable<SpaceData> SpaceDataList { get; init; }
	public IDictionary<Guid, SpaceData> SpaceDataDict { get; init; }
	public IEnumerable<SpaceView> SpaceViewList { get; init; }
	public IDictionary<Guid, SpaceView> SpaceViewDict { get; init; }

	public string SpaceColor
	{
		get
		{
			if(SpaceRouteId is null || Space is null) return ThemeColor.ToAttributeValue();

			return Space.Color.ToAttributeValue();
		}
	}

	public string SpaceBackgkroundColor => $"{SpaceColor}15";

	public string SpaceBorderColor => $"{SpaceColor}25";

	public string SpaceButtonColor => $"{SpaceColor}35";

	public string SpaceSidebarStyle => $"background-color:{SpaceBackgkroundColor} !important; border-color:{SpaceBorderColor} !important";

}
