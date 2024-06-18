namespace WebVella.Tefter.Migrations;

public interface ITefterAddOnMigration
{
	public void MigrateStructure(Guid appId,DatabaseBuilder db);

	public void MigrateData(Guid appId, IDatabaseService dbService);
}
