namespace WebVella.Tefter.Web.Utils;
using SystemColor = System.Drawing.Color;
public partial class TfConstants
{
	public static DesignThemeModes DefaultThemeMode { get; } = DesignThemeModes.System;
	public static OfficeColor DefaultThemeColor { get; } = OfficeColor.Booking;

	public static DesignThemeModes AdminThemeMode { get; } = DesignThemeModes.System;
	public static OfficeColor AdminThemeColor { get; } = OfficeColor.Office;
	public static IconSize IconSize { get; } = IconSize.Size20;
	public static IconVariant IconVariant { get; } = IconVariant.Regular;

	public static string AdminColor { get => AdminThemeColor.ToAttributeValue(); }

	public static SystemColor AdminColorObject
	{
		get
		{
			return (SystemColor)System.Drawing.ColorTranslator.FromHtml(AdminColor);
		}
	}

	public static string AdminBackgkroundColor => $"{AdminColor}25";
	public static string AdminBorderColor => $"{AdminColor}";
	public static string AdminSidebarStyle => $"background-color:{AdminBackgkroundColor} !important; border-color:{AdminBorderColor} !important";

	public static string DialogWidthDefault = $"500px";
	public static string DialogWidthSmall = $"300px";
	public static string DialogWidthLarge = $"800px";
	public static string DialogWidthExtraLarge = $"1140px";
}
