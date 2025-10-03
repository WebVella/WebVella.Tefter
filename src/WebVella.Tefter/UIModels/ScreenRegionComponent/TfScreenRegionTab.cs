namespace WebVella.Tefter.Models;

public record TfScreenRegionTab
{
	public string Slug { get; private set; } = null!;
	public string Label { get; private set; } = null!;
	public string? FluentIconName { get; private set; } = null;

	public TfScreenRegionTab() { }

	public TfScreenRegionTab(string slug, string label, string? fluentIconName = null)
	{
		Slug = slug;
		Label = label;
		FluentIconName = fluentIconName;
	}
}