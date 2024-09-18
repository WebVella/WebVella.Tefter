using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.UseCases.Models;

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
}

