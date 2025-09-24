namespace WebVella.Tefter.Models;

public record TfDataset
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	[Obsolete("Property is not used anymore and will be removed")]
	public Guid SpaceId { get; set; }
	public string Name { get; set; }
	[Obsolete("Property is not used anymore and will be removed")]
	public short Position { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
	public ReadOnlyCollection<TfDatasetIdentity> Identities { get; internal set; }
}

public class TfCreateDataset
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string Name { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
	public List<TfDatasetIdentity> Identities { get; set; } = new();
}

public class TfUpdateDataset
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public Guid DataProviderId { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();

	public TfUpdateDataset() { }

	public TfUpdateDataset(TfDataset ds)
	{
		Id = ds.Id;
		Name = ds.Name;
		DataProviderId = ds.DataProviderId;
		Filters = ds.Filters;
		Columns = ds.Columns;
		SortOrders = ds.SortOrders;
	}
}

public class TfAvailableDatasetColumn
{
	public string DbName { get; set; }
	public TfDatabaseColumnType DbType { get; init; }
}

public class TfDatasetAsOption
{
	public Guid Id { get; set; }
	public string SpaceName { get; set; }
	public string Name { get; set; }
}


[DboCacheModel]
[TfDboModel("tf_dataset")]
internal class TfDatasetDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("filters_json")]
	public string FiltersJson { get; set; }

	[TfDboModelProperty("columns_json")]
	public string ColumnsJson { get; set; }

	[TfDboModelProperty("sort_orders_json")]
	public string SortOrdersJson { get; set; }
}
