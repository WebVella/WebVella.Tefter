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
		public TfSystemMigration Instance { get; set; }

	}

	private record AddOnMigrationMeta
	{
		public Version Version { get; set; }
		public Guid ApplicationId { get; set; }
		public string MigrationClassName { get; set; }
		public ITfApplicationMigration Instance { get; set; }
	}
}
