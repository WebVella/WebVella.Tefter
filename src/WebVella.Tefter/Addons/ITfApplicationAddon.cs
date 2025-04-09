namespace WebVella.Tefter.Addons;

public interface ITfApplicationAddon : ITfAddon
{
	public void OnStart();
	public void OnRegisterDependencyInjections(IServiceCollection services);
}


public class TfApplicationAddonMeta : ITfAddonMeta
{
	public new ITfApplicationAddon Instance { get; init; }
}
