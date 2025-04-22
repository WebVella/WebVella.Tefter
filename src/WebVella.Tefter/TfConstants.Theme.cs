namespace WebVella.Tefter;

using SystemColor = System.Drawing.Color;
public partial class TfConstants
{
	public static DesignThemeModes DefaultThemeMode { get; } = DesignThemeModes.System;
	public static TfColor DefaultThemeColor { get; } = TfColor.Emerald500;

	public static DesignThemeModes AdminThemeMode { get; } = DesignThemeModes.System;
	public static TfColor AdminThemeColor { get; } = TfColor.Red500;
	public static IconSize IconSize { get; } = IconSize.Size20;
	public static IconVariant IconVariant { get; } = IconVariant.Regular;

	public static string AdminColor { get => AdminThemeColor.ToColorString(); }

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
	public static string DialogWidthFullScreen = $"99vw";
}
