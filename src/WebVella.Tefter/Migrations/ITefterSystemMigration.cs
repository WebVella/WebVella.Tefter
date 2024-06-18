namespace WebVella.Tefter.Migrations;

internal interface ITefterSystemMigration
{
	public void MigrateStructure(DatabaseBuilder dbBuilder);
	public void MigrateData(IDatabaseService dbService, IDboManager dboManager);
}
