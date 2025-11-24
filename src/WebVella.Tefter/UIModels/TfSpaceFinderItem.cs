namespace WebVella.Tefter.Models;

public record TfSpaceFinderItem
{
	public TfSpace Space { get; set; } = null!;
	public int Index { get; set; }
	public bool Bookmarked { get; set; } = false;
	public bool Current { get; set; } = false;
	public string? Color { get; set; } = null;
	[JsonIgnore]
	public Action OnClick { get; set; }
	
	[JsonIgnore]
	public Action OnBookmark { get; set; }	

	public TfSpaceFinderItem(
		TfSpace space, 
		int index, 
		bool bookmarked,
		bool current,
		string? color,
		Action onClick,
		Action onBookmark)
	{
		Space = space;
		Index = index;
		Bookmarked = bookmarked;
		Current = current;
		Color = color;
		OnClick = onClick;
		OnBookmark = onBookmark;
	}
}
