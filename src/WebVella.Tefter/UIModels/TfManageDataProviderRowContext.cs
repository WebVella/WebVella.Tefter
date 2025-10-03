namespace WebVella.Tefter.Models;

public record TfManageDataProviderRowContext
{
	public TfDataProvider Provider { get; set; } = null!;
	public TfDataTable Data { get; set; } = null!;
	public Guid? RowId { get; set; } = null;
}
