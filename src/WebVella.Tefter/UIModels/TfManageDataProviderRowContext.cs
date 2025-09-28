namespace WebVella.Tefter.Models;

public record TfManageDataProviderRowContext
{
	public TfDataProvider Provider { get; set; } = default!;
	public TfDataTable Data { get; set; } = default!;
	public Guid? RowId { get; set; } = null;
}
