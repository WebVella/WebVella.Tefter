namespace WebVella.Tefter.UI.Components;
public partial class TucPageHeader : ComponentBase
{
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? Area { get; set; }
	[Parameter] public RenderFragment Toolbar { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public string? Class { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public Icon? Icon { get; set; } = null;
	[Parameter] public TfColor? IconColor { get; set; } = null;
	private string _cssClass { get => $"tf-page-header {Class}"; }

	private TfColor _iconForegroundColor
	{
		get
		{
			if (IconColor is null) return TfColor.White;

			var colorInfo = IconColor.Value.GetAttribute();
			if (colorInfo.UseWhiteForeground) return TfColor.White;
			return TfColor.Slate950;
		}
	}
	private TfColor _iconBackgroundColor { get => IconColor ?? TfColor.Slate600; }
}