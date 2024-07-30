namespace WebVella.Tefter;
public class TfDataTableQuery
{
	public const int DEFAULT_PAGE = 1;
	
	public const int DEFAULT_PAGE_SIZE = 10;

	public Guid DataProviderId { get; set; }
	public int? Page { get; set; } = null;
	public int? PageSize { get; set; } = null;
	public string Search { get; set; } = null;
	public bool ExcludeSharedColumns { get; set; } = false;

	
}
