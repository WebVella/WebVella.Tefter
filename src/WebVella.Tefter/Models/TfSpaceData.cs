namespace WebVella.Tefter.Models;

public class TfSpaceData
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	public short Position { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
}

public class TfAvailableSpaceDataColumn
{
	public string DbName { get; set; }
	public TfDatabaseColumnType DbType { get; init; }
}

public class TfSpaceDataAsOption
{
	public Guid Id { get; set; }
	public string SpaceName { get; set; }
	public string Name { get; set; }
}


[DboCacheModel]
[TfDboModel("space_data")]
internal class TfSpaceDataDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("filters_json")]
	public string FiltersJson { get; set; }

	[TfDboModelProperty("columns_json")]
	public string ColumnsJson { get; set; }

	[TfDboModelProperty("sort_orders_json")]
	public string SortOrdersJson { get; set; }
}
