namespace WebVella.Tefter.Models;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TfScreenRegionComponentAttribute : Attribute
{
	public ScreenRegion ScreenRegion { get; init; }

	public int Position { get; init; }

	public string UrlSlug { get; init; } = null;

	public TfScreenRegionComponentAttribute(
		ScreenRegion ScreenRegion,
		int Position,
		string UrlSlug = null )
	{
		this.ScreenRegion = ScreenRegion;
		this.Position = Position;
		this.UrlSlug = UrlSlug;
	}
}
