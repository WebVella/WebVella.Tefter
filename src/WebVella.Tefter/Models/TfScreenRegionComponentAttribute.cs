namespace WebVella.Tefter.Models;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TfScreenRegionComponentAttribute : Attribute
{
	public ScreenRegion ScreenRegion { get; init; }

	public int Position { get; init; }

	public TfScreenRegionComponentAttribute(
		ScreenRegion ScreenRegion,
		int Position)
	{
		this.ScreenRegion = ScreenRegion;
		this.Position = Position;
	}
}
