namespace WebVella.Tefter.Web.Models;

public record UserSession
{
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool SidebarExpanded { get; init; } = true;

	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }

	public string DataHashId { get; init; }
	public IEnumerable<Space> SpaceList { get; init; }
	public IDictionary<Guid,Space> SpaceDict { get; init; }
	public IEnumerable<SpaceData> SpaceDataList { get; init; }
	public IDictionary<Guid, SpaceData> SpaceDataDict { get; init; }
	public IEnumerable<SpaceView> SpaceViewList { get; init; }
	public IDictionary<Guid, SpaceView> SpaceViewDict { get; init; }
}
