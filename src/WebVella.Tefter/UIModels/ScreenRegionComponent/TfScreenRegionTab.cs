namespace WebVella.Tefter.Models;

public record TfScreenRegionTab
{
	public string UrlSlug { get;  private set; } = null!;
	public string Label { get;  private set; } = null!;
	public string? FluentIconName { get;  private set; } = null;
}
