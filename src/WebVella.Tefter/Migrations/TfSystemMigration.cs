namespace WebVella.Tefter.Migrations;

internal abstract class TfSystemMigration
{
	public virtual void MigrateStructure(DatabaseBuilder dbBuilder) { }

	public virtual Task MigrateDataAsync(IServiceProvider serviceProvider) {  return Task.CompletedTask; }
}
