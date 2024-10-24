namespace WebVella.Tefter;

public interface ITfApplication
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public void OnStart();
	public void OnRegisterDependencyInjections(IServiceCollection services);
}
