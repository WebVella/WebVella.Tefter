namespace WebVella.Tefter.Migrations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TefterSystemMigrationAttribute : Attribute
{
	public Version Version { get; private set; }

	public TefterSystemMigrationAttribute(string Version)
	{
		this.Version = new Version(Version);
	}
}
