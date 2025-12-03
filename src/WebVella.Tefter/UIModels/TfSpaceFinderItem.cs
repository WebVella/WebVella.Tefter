namespace WebVella.Tefter.Models;

public record TfSpaceFinderItem
{
	public TfSpace Space { get; set; } = null!;
	public int Index { get; set; }
	public bool Current { get; set; } = false;
	public string? Color { get; set; } = null;
	[JsonIgnore]
	public Action OnClick { get; set; }
	
	public TfSpaceFinderItem(
		TfSpace space, 
		int index, 
		bool current,
		string? color,
		Action onClick,
		Action onBookmark)
	{
		Space = space;
		Index = index;
		Current = current;
		Color = color;
		OnClick = onClick;
	}
}
