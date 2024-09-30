namespace WebVella.Tefter.Migrations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfSystemMigrationAttribute : Attribute
{
	public Version Version { get; private set; }

	public TfSystemMigrationAttribute(string Version)
	{
		this.Version = new Version(Version);
	}
}
