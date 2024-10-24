namespace WebVella.Tefter;

public interface ITfSpaceNodeComponent
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public ComponentDisplayMode DisplayMode { get; }
	public object GetData(IServiceProvider serviceProvider);
}


public class TfSpaceNodeComponentMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public ComponentDisplayMode DisplayMode { get { return Instance.DisplayMode; } }
	public ITfSpaceNodeComponent Instance { get; init; }
}