namespace WebVella.Tefter.Models;

public record TfNavigationMenu
{
	public string Uri { get; set; } = String.Empty;
	public string SpaceName { get; set; } = default!;
	public TfColor SpaceColor { get; set; } = TfConstants.DefaultThemeColor;
	public Icon SpaceIcon { get; set; } = default!;
	public List<TfMenuItem> Menu { get; set; } = new();
}
