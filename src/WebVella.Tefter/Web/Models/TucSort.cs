using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Web.Models;

public record TucSort
{
	[JsonPropertyName("db")]
	public string ColumnName { get; set; }
	[JsonPropertyName("dr")]
	public TucSortDirection Direction { get; set; } = TucSortDirection.ASC;
	public TucSort() { }
	public TucSort(TfSort model)
	{
		ColumnName = model.ColumnName;
		Direction = model.Direction.ConvertSafeToEnum<TfSortDirection, TucSortDirection>();
	}
	public TfSort ToModel()
	{
		return new TfSort
		{
			ColumnName = ColumnName,
			Direction = Direction.ConvertSafeToEnum<TucSortDirection, TfSortDirection>()
		};
	}

	public TucSort(TucSortQuery model)
	{
		ColumnName = model.Name;
		Direction = (TucSortDirection)model.Direction;
	}
	public TucSortQuery ToQuery()
	{
		return new TucSortQuery
		{
			Name = ColumnName,
			Direction = (int)Direction
		};
	}
}

