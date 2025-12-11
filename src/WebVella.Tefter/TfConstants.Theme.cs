namespace WebVella.Tefter;

public partial class TfConstants
{
	public static TfColor DefaultThemeColor { get; } = TfColor.Teal500;
	public static IconSize IconSize { get; } = IconSize.Size20;
	public static IconVariant IconVariant { get; } = IconVariant.Regular;

	public static readonly string DialogWidthSmall = "300px";
	public static readonly string DialogWidthLarge = "800px";
	public static readonly string DialogWidthExtraLarge = "1140px";
	public static readonly string DialogWidthContentScreen = "calc(90vw - 200px)";
	public static readonly string DialogWidthFullScreen = "99vw";
}