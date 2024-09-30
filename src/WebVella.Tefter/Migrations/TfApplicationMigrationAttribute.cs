namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfApplicationMigrationAttribute : Attribute
{
	public Guid ApplicationId { get; private set; }
	public Version Version { get; private set; }

	public TfApplicationMigrationAttribute(string ApplicationId, string Version)
	{
		this.ApplicationId = new Guid(ApplicationId);
		this.Version = new Version(Version);
	}
}
