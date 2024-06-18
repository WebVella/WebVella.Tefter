namespace WebVella.Tefter.Migrations;

internal interface ITefterSystemMigration
{
	public void Migrate(IDatabaseManager dbManager, IDboManager dboManager);
}
