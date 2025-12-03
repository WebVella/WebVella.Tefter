namespace WebVella.Tefter.UI.Components;

public partial class TucTileCard : TfBaseComponent
{
	[Parameter] public string? Url { get; set; }
	[Parameter] public string Title { get; set; } = "Title";
	[Parameter] public string Subtitle { get; set; } = "subTitle";
	[Parameter] public string Footer { get; set; } = "footer";
	[Parameter] public string? FluentIconName { get; set; } = null;
	[Parameter] public TfColor? Color { get; set; } = null;
	[Parameter] public List<TfMenuItem> Menu { get; set; } = new();
}