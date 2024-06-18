namespace WebVella.Tefter.Web.Utils;
using SystemColor = System.Drawing.Color;
public partial class TfConstants
{
	public static int PageSize { get; set; } = 15;
	public static int CardPageSize { get; set; } = 20;

	public static OfficeColor AdminOfficeColor { get; set; } = OfficeColor.Office;
	public static string AdminColor { get => AdminOfficeColor.ToAttributeValue(); }

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

}
