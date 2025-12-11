namespace WebVella.Tefter.Models;
public record TfSpaceViewMeta
{
	public Dictionary<Guid, TfSpaceViewRowPresentationMeta> RowMeta { get;set;} = new();
	public Dictionary<Guid, TfSpaceViewColumnPresentationMeta> ColumnsMeta { get; set; } = new();
	public Dictionary<Guid, Dictionary<Guid, TfSpaceViewColumnBase>> RegionContextDict { get; set; } = new();
	public Dictionary<string, string> QueryNameToColumnNameDict { get; set; } = new();
	public Dictionary<Guid, Dictionary<string, Tuple<TfColor?, TfColor?>>> RowColoringCacheDictionary { get; set; } = new();
	public Dictionary<string, object> ContextViewData { get; set; } = new();
}
