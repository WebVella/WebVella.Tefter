namespace WebVella.Tefter.Demo.Models;

public record StateActiveSpaceDataChangedEventArgs
{
	public Space Space { get; set; }
	public SpaceItem SpaceItem { get; set; }
}
