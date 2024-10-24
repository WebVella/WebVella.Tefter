namespace WebVella.Tefter;

public interface ISpaceNodeComponent
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public Type SettingsComponentType { get; }
}
