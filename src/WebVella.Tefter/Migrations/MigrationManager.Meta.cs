namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	private record SystemMigrationMeta
	{
		public Version Version { get; set; }
		public string MigrationClassName { get; set; }
		public TefterSystemMigration Instance { get; set; }

	}

	private record AddOnMigrationMeta
	{
		public Version Version { get; set; }
		public Guid AddOnTypeId { get; set; }
		public string MigrationClassName { get; set; }
		public ITefterAddOnMigration Instance { get; set; }
	}
}
