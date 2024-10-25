namespace WebVella.Tefter;

public interface ITfSpaceNodeComponent
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public ITfSpaceNodeComponentContext Context { get; set; }
	public object GetData(IServiceProvider serviceProvider);
}

public interface ITfSpaceNodeComponentContext
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public string Icon { get; set; }
	public string ComponentType { get; set; }
	public string ComponentSettingsJson { get; set; }
	public ComponentDisplayMode DisplayMode { get; set; }
}

public class TfSpaceNodeComponentMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public ComponentDisplayMode DisplayMode { get { return Instance.DisplayMode; } }
	public Type ComponentType { get; init; }
	public ITfSpaceNodeComponent Instance { get; init; }
}