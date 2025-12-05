using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Models;

public record TfDashboardItem
{
	public Guid Id { get; set; }
	public DateTime CreatedOn { get; set; }
	public string? Title { get; set; }
	public string? SubTitle { get; set; }
	public string? Description { get; set; }
	public string? Footer { get; set; }
	public string? Icon { get; set; }
	public string? IconText { get; set; }
	public TfColor? Color { get; set; }
	public string? Url { get; set; }
	public List<TfMenuItem> Menu { get; set; } = new();

	public TfDashboardItem() { }
	public TfDashboardItem(TfBookmark bookmark, 
		NavigationManager navigator,
		string tagLinkTitle,
		List<TfMenuItem> menu)
	{
		Id = bookmark.Id;
		CreatedOn = bookmark.CreatedOn;
		Title = bookmark.Name;
		if (bookmark.Space is not null && bookmark.SpacePage is not null)
		{
			SubTitle = $"{bookmark.Space.Name} : {bookmark.SpacePage.Name}";
			Icon = bookmark.SpacePage.FluentIconName;
			Color = bookmark.Space.Color;
		}

		IconText = String.IsNullOrWhiteSpace(bookmark.Url) ? "PAGE" : "URL";
		Url = bookmark.GetUrl();
		var result = new List<string>();
		foreach (var tag in bookmark.Tags)
		{
			result.Add($"<a href='{navigator.GetLocalAndQueryUrl().ApplyChangeToUrlQuery(TfConstants.TagQueryName, tag.Label)}' title='{tagLinkTitle}'>#{tag.Label}</a>");
		}

		if (result.Count > 0)
			Footer = String.Join(", ", result);

		Menu = menu;
	}



}
