namespace WebVella.Tefter;

public abstract class TfApplicationBase
{
	public abstract Guid Id { get; }
	public abstract string Name { get; }
	public abstract string Description { get; }

	public virtual void OnStart()
	{
	}

	public virtual void OnRegisterDependencyInjections(IServiceCollection services)
	{
	}
}
