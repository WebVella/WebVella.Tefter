namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	[TfDboModel("tf_migration")]
	private record Migration
	{
		[TfDboModelProperty("id")]
		public Guid Id { get; init; }

		[TfDboModelProperty("application_id")]
		public Guid? ApplicationId { get; init; }

		[TfDboModelProperty("migration_class_name")]
		public string MigrationClassName { get; init; }

		[TfDboModelProperty("executed_on")]
		[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
		public DateTime ExecutedOn { get; init; }

		[TfDboModelProperty("major_ver")]
		[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
		public int MajorVer { get; init; }

		[TfDboModelProperty("minor_ver")]
		[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
		public int MinorVer { get; init; }

		[TfDboModelProperty("build_ver")]
		[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
		public int BuildVer { get; init; }

		[TfDboModelProperty("revision_ver")]
		[TfDboTypeConverter(typeof(TfIntegerPropertyConverter))]
		public int RevisionVer { get; init; }

		public Version Version => new Version(MajorVer, MinorVer, BuildVer, RevisionVer);
	}

}
