namespace WebVella.Tefter.Addons;

public interface ITfApplicationAddon : ITfAddon
{
	public void OnStart();
	public void OnRegisterDependencyInjections(IServiceCollection services);
}
