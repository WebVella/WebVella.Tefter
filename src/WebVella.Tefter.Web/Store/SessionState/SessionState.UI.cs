namespace WebVella.Tefter.Web.Store.SessionState;
using System.Drawing;
using SystemColor = System.Drawing.Color;
public partial record SessionState
{
	//Localization
	public CultureOption CultureOption { get; init; }
	//UI
	public DesignThemeModes ThemeMode { get; init; } = DesignThemeModes.System;
	public OfficeColor ThemeColor { get; init; } = OfficeColor.Excel;
	public bool SidebarExpanded { get; init; } = true;

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
}
