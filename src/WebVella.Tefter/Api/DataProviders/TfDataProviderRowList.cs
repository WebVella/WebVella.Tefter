namespace WebVella.Tefter;

public class TfDataProviderRowList : List<TfDataProviderDataRow>
{
	public int? Page { get; set; } = null;
	public int? PageSize { get; set; } = null;
}
