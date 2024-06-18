namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TefterAddOnMigrationAttribute : Attribute
{
	public Guid AddOnTypeId { get; private set; }
	public Version Version { get; private set; }

	public TefterAddOnMigrationAttribute(string AddOnTypeId, string Version)
	{
		this.AddOnTypeId = new Guid(AddOnTypeId);
		this.Version = new Version(Version);
	}
}
