namespace WebVella.Tefter.Web.Utils;
using SystemColor = System.Drawing.Color;
public partial class TfConstants
{
	public static DesignThemeModes DefaultThemeMode { get; set; } = DesignThemeModes.System;
	public static OfficeColor DefaultThemeColor { get; set; } = OfficeColor.Excel;

	public static DesignThemeModes AdminThemeMode { get; set; } = DesignThemeModes.System;
	public static OfficeColor AdminThemeColor { get; set; } = OfficeColor.Office;

	public static string AdminColor { get => AdminThemeColor.ToAttributeValue(); }

	public static SystemColor AdminColorObject
	{
		get
		{
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(AdminColor);
		}
	}

	public static string AdminBackgkroundColor => $"{AdminColor}15";

	public static string AdminBorderColor => $"{AdminColor}50";

	public static string AdminBackgroundAccentColor => $"{AdminColor}35";

	public static string AdminSidebarStyle => $"background-color:{AdminBackgkroundColor} !important; border-color:{AdminBorderColor} !important";
	public static string AdminPagebarStyle => $"background-color:{AdminBorderColor} !important;";

	public static string DialogWidthDefault = $"500px";
	public static string DialogWidthSmall = $"300px";
	public static string DialogWidthLarge = $"800px";
	public static string DialogWidthExtraLarge = $"1140px";
}
