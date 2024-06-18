
namespace WebVella.Tefter.Core.Migrations;

[TefterAddOnMigration(Constants.TEFTER_CORE_ADDON_ID, "2024.6.18.1")]
public class Migration_2024_06_18_01 : ITefterAddOnMigration
{
	public void MigrateData(Guid appId, IDatabaseService dbService)
	{
	}

	public void MigrateStructure(Guid appId, DatabaseBuilder db)
	{
	}
}
