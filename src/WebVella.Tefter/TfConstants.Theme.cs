namespace WebVella.Tefter;

using SystemColor = System.Drawing.Color;
public partial class TfConstants
{
	public static TfColor AdminColor { get; } = TfColor.Red700;
	public static TfColor DefaultThemeColor { get; } = TfColor.Teal500;
	public static IconSize IconSize { get; } = IconSize.Size20;
	public static IconVariant IconVariant { get; } = IconVariant.Regular;

	public static string DialogWidthDefault = $"500px";
	public static string DialogWidthSmall = $"300px";
	public static string DialogWidthLarge = $"800px";
	public static string DialogWidthExtraLarge = $"1140px";
	public static string DialogWidthFullScreen = $"99vw";
}
