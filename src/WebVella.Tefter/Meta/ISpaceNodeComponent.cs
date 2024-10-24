namespace WebVella.Tefter;

public interface ISpaceNodeComponent
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public Type SettingsComponentType { get; }
	public object GetData(IServiceProvider serviceProvider);
}


public class TfSpaceNodeComponentMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public Type SettingsComponentType { get { return Instance.SettingsComponentType; } }
	public ISpaceNodeComponent Instance { get; init; }
}