namespace WebVella.Tefter;

public class TfSpaceData
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public short Position { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new ();
	public List<string> Columns { get; set; } = new();
}

public class TfAvailableSpaceDataColumn
{
	public string DbName { get; set; }
	public DatabaseColumnType DbType { get; init; }
}


[DboCacheModel]
[DboModel("space_data")]
internal class TfSpaceDataDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[DboModelProperty("space_id")]
	public Guid SpaceId { get; set; }
	
	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("position")]
	public short Position { get; set; }

	[DboModelProperty("filters_json")]
	public string FiltersJson { get; set; }

	[DboModelProperty("columns_json")]
	public string ColumnsJson { get; set; }
}
