namespace WebVella.Tefter.Migrations;

internal interface ITefterSystemMigration
{
	public void MigrateStructure(DatabaseBuilder dbBuilder);
	public void MigrateData(IDboManager dboManager);
}
