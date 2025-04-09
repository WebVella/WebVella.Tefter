namespace WebVella.Tefter.Addons;

public interface ITfAddon
{
	public Guid Id { get; init;}
	public string Name { get; init;}
	public string Description { get; init;}
	public string FluentIconName { get; init;}
}

public abstract class ITfAddonMeta
{
	public virtual ITfAddon Instance { get; init; }
}