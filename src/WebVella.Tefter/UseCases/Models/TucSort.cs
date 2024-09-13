using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.UseCases.Models;

public record TucSort
{
	public string DbName { get; set; }
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

