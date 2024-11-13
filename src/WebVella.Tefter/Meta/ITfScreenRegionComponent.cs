namespace WebVella.Tefter;

public interface ITfScreenRegionComponent
{
	public Guid Id { get; }
	public TfScreenRegion ScreenRegion { get; }
	public int Position { get; }
	public string Name { get; }
	public string FluentIconName { get; }
}

public class TfScreenRegionComponentMeta
{
	public Guid Id { get { return Instance.Id; } }
	public TfScreenRegion ScreenRegion { get { return Instance.ScreenRegion; } }
	public int Position { get { return Instance.Position; } }
	public string Name { get { return Instance.Name; } }
	public string FluentIconName { get { return Instance.FluentIconName; } }
	public Type ComponentType { get; init; }
	public ITfScreenRegionComponent Instance { get; init; }
}

//public class TfScreenRegionComponentMeta
//{
//	public TfScreenRegion ScreenRegion { get; init; } 
//	public int Position { get; init; }
//	public string Name { get; init; }
//	public string UrlSlug { get; init; }
//	public Type ComponentType { get; init; }
//}