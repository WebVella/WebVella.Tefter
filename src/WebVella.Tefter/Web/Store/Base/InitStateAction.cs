namespace WebVella.Tefter.Web.Store;

public record InitStateAction : TfBaseAction
{
	public TucUser User { get; }
	public TucCultureOption Culture { get; }
	public List<TucSpace> UserSpaces { get; }
	public DesignThemeModes ThemeMode { get; }
	public OfficeColor ThemeColor { get; }
	public bool SidebarExpanded { get; }

	internal InitStateAction(
		TfBaseComponent component,
		TucUser user,
		TucCultureOption culture,
		List<TucSpace> userSpaces,
		DesignThemeModes themeMode,
		OfficeColor themeColor,
		bool sidebarExpanded)
	{
		Component = component;
		User = user;
		UserSpaces = userSpaces;
		Culture = culture;
		ThemeMode = themeMode;
		ThemeColor = themeColor;
		SidebarExpanded = sidebarExpanded;
	}
}
