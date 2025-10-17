namespace WebVella.Tefter.Models;

public record TfSpaceViewColumnDataChange
{
	public Guid RowId { get; set; }
	public Dictionary<string, object?> DataChange { get; set; } = new();

}


