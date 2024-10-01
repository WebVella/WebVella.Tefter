using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.Web.Models;

public record TucSort
{
	[JsonPropertyName("db")]
	public string DbName { get; set; }
	[JsonPropertyName("dr")]
	public TucSortDirection Direction { get; set; } = TucSortDirection.ASC;
	public TucSort() { }
	public TucSort(TfSort model)
	{
		DbName = model.DbName;
		Direction = model.Direction.ConvertSafeToEnum<TfSortDirection, TucSortDirection>();
	}
	public TfSort ToModel()
	{
		return new TfSort
		{
			DbName = DbName,
			Direction = Direction.ConvertSafeToEnum<TucSortDirection, TfSortDirection>()
		};
	}

	public TucSort(TucSortQuery model)
	{
		DbName = model.Name;
		Direction = (TucSortDirection)model.Direction;
	}
	public TucSortQuery ToQuery()
	{
		return new TucSortQuery
		{
			Name = DbName,
			Direction = (int)Direction
		};
	}
}

