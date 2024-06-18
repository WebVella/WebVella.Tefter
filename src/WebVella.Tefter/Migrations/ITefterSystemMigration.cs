namespace WebVella.Tefter.Migrations;

internal interface ITefterSystemMigration
{
	public void Migrate(IDatabaseService dbService, DatabaseBuilder db);
}
