namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	[DboModel("migration")]
	internal record Migration
	{
		[DboModelProperty("id")]
		public Guid Id { get; init; }

		[DboModelProperty("addon_id")]
		public Guid? AddOnId { get; init; }

		[DboModelProperty("version")]
		public Version Version { get; init; }

		[DboModelProperty("migration_class_name")]
		public string MigrationClassName { get; init; }

		[DboModelProperty("created_on")]
		[DboTypeConverter(typeof(LegacyDateTimePropertyConverter))]
		public DateTime ExecutedOn { get; init; }
	}
}
