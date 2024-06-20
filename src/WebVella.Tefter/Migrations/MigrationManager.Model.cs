namespace WebVella.Tefter.Migrations;

internal partial interface IMigrationManager
{
}

internal partial class MigrationManager : IMigrationManager
{
	[DboModel("migration")]
	private record Migration
	{
		[DboModelProperty("id")]
		public Guid Id { get; init; }

		[DboModelProperty("addon_id")]
		public Guid? AddOnId { get; init; }

		[DboModelProperty("migration_class_name")]
		public string MigrationClassName { get; init; }

		[DboModelProperty("executed_on")]
		[DboTypeConverter(typeof(DateTimePropertyConverter))]
		public DateTime ExecutedOn { get; init; }

		[DboModelProperty("major_ver")]
		[DboTypeConverter(typeof(IntegerPropertyConverter))]
		public int MajorVer { get; init; }

		[DboModelProperty("minor_ver")]
		[DboTypeConverter(typeof(IntegerPropertyConverter))]
		public int MinorVer { get; init; }

		[DboModelProperty("build_ver")]
		[DboTypeConverter(typeof(IntegerPropertyConverter))]
		public int BuildVer { get; init; }

		[DboModelProperty("revision_ver")]
		[DboTypeConverter(typeof(IntegerPropertyConverter))]
		public int RevisionVer { get; init; }

		public Version Version => new Version(MajorVer, MinorVer, BuildVer, RevisionVer);
	}

}
