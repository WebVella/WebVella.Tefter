namespace WebVella.Tefter.Migrations;

public interface ITefterAddOnMigration
{
	public void Migrate(Guid appId,DatabaseBuilder db);
}
