namespace WebVella.Tefter;

public interface ITfApplication
{
	public Guid Id { get; }
	public string Name { get; }
	public string Description { get; }
	public void OnStart();
	public void OnRegisterDependencyInjections(IServiceCollection services);
}


public class TfApplicationMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
	public ITfApplication Instance { get; init; }
}
