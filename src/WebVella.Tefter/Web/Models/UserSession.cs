namespace WebVella.Tefter.Web.Models;

public record UserSession
{
	public Guid UserId { get; init; }
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool SidebarExpanded { get; init; } = true;
	public string CultureCode { get; init; }
	public string DataHashId { get; init; }
	public Space Space { get; init; }
	public SpaceData SpaceData { get; init; }
	public SpaceView SpaceView { get; init; }
	public IDictionary<Guid, SpaceData> SpaceDataDict { get; init; }
	public IDictionary<Guid, SpaceView> SpaceViewDict { get; init; }
	public List<MenuItem> SpaceNav { get; init; }
}
