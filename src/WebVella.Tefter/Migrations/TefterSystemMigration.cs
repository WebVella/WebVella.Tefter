namespace WebVella.Tefter.Migrations;

internal abstract class TefterSystemMigration
{
	public abstract void MigrateStructure(DatabaseBuilder dbBuilder);

	public abstract Task MigrateDataAsync(IServiceProvider serviceProvider);	
}
