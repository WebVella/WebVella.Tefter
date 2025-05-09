namespace WebVella.Tefter.Addons;

public interface ITfAddon
{
	public Guid AddonId { get; init;}
	public string AddonName { get; init;}
	public string AddonDescription { get; init;}
	public string AddonFluentIconName { get; init;}
}

public abstract class ITfAddonMeta
{
	public virtual ITfAddon Instance { get; init; }
}