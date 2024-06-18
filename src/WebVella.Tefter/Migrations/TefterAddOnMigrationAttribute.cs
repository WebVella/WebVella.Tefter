namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TefterAddOnMigrationAttribute : Attribute
{
	public Guid AppId { get; private set; }
	public Version Version { get; private set; }

	public TefterAddOnMigrationAttribute(string AppId, string Version)
	{
		this.AppId = new Guid(AppId);
		this.Version = new Version(Version);
	}
}
