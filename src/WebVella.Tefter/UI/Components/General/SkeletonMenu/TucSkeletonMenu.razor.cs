namespace WebVella.Tefter.UI.Components;
public partial class TucSkeletonMenu : TfBaseComponent
{
	[Parameter] public bool HasIcon { get; set; } = false;
	[Parameter] public string? Class { get; set; }
	[Parameter] public string? BackgroundColor { get; set; }
	[Parameter] public bool IsAside { get; set; } = false;

	private string _iconStyle
	{
		get
		{
			var style = string.Empty;
			if (!IsAside)
				style += "display:inline-block;";
			if (!String.IsNullOrWhiteSpace(BackgroundColor))
				style += $"background-color:{BackgroundColor}";
			return style;
		}
	}
	private string _menuStyle
	{
		get
		{
			var style = string.Empty;
			if (!IsAside)
				style += "display:inline-block;position:relative;top:2px;";

			if (!String.IsNullOrWhiteSpace(BackgroundColor))
			{
				style += $"background-color:{BackgroundColor}";
			}
			return style;
		}
	}
}