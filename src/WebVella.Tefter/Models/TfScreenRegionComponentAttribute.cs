namespace WebVella.Tefter.Models;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TfScreenRegionComponentAttribute : Attribute
{
	public TfScreenRegion ScreenRegion { get; init; }

	public int Position { get; init; }
	public string Name { get; init; } = null;
	public string UrlSlug { get; init; } = null;

	public TfScreenRegionComponentAttribute(
		TfScreenRegion ScreenRegion,
		int Position,
		string Name = null,
		string UrlSlug = null )
	{
		this.ScreenRegion = ScreenRegion;
		this.Position = Position;
		this.Name = Name;
		this.UrlSlug = UrlSlug;
	}
}
