namespace WebVella.Tefter.Models;

public record TfDataset
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public string Name { get; set; }
	public List<TfFilterBase> Filters { get; set; } = new();
	public List<string> Columns { get; set; } = new();
	public List<TfSort> SortOrders { get; set; } = new();
	public ReadOnlyCollection<TfDatasetIdentity> Identities { get; internal set; }

	public int ColumnsCount
	{
		get
		{
			var count = Columns.Count;
			if (Identities is not null)
				foreach (var identity in Identities)
					count += identity.Columns.Count;
			return count;
		}
	}

	public int FiltersCount
	{
		get
		{
			var filterIds = new List<Guid>();
			if(Filters is not null) 
				Filters.ForEach(x=> GetFilterCount(x,filterIds));

			return filterIds.Count;
		}
	}

	public int SortOrdersCount
	{
		get
		{
			return SortOrders.Count;
		}
	}

	private void GetFilterCount(TfFilterBase filter, List<Guid> filterIds)
	{
		if (filter is TfFilterAnd)
		{
			var model = (TfFilterAnd)filter;
			foreach (var item in model.Filters)
			{
				GetFilterCount(item,filterIds);
			}
		}
		else if (filter is TfFilterAnd)
		{
			var model = (TfFilterAnd)filter;
			foreach (var item in model.Filters)
			{
				GetFilterCount(item, filterIds);
			}
		}
		else
		{
			filterIds.Add(filter.Id);
		}
	}
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
