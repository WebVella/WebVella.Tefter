namespace WebVella.Tefter.Web.Models;

public record TucMenuItem
{
	public string Id { get; set; } //should not be just guid as there are problems
	public string Text { get; set; }
	public string Description { get; set; }
	public Icon IconCollapsed { get; set; }
	public Icon IconExpanded { get; set; }
	public TfColor? IconColor { get; set; }
	public Icon Icon
	{
		get => Expanded
			? (IconColor is null ? IconExpanded : IconExpanded.WithColor(IconColor.GetAttribute().Value))
			: (IconColor is null ? IconCollapsed : IconCollapsed.WithColor(IconColor.GetAttribute().Value));
	}
	public string Url { get; set; }
	public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
	public bool Expanded { get; set; } = false;
	public bool Selected { get; set; } = false;
	public object Data { get; set; }
	public List<TucMenuItem> Nodes { get; set; } = new();
	public ElementReference Reference { get; set; }

	[JsonIgnore]
	public Action OnClick { get; set; }

	[JsonIgnore]
	public Action OnExpand { get; set; }
}
