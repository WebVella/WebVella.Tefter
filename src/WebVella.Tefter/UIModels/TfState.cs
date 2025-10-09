namespace WebVella.Tefter.Models;

public record TfState
{
	public string Uri { get; set; } = String.Empty;
	public TfNavigationState NavigationState { get; set; } = null!;
	public TfUser User { get; set; } = null!;
	public List<TfBookmark> UserSaves { get; set; } = new();
	public TfSpace? Space { get; set; } = null;

	public List<TfSpacePage>? SpacePages { get; set; } = null;
	public TfSpacePage? SpacePage { get; set; } = null;
	public List<TfMenuItem> Breadcrumb { get; set; } = new();
	public List<TfMenuItem> Menu { get; set; } = new();
}