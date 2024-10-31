namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucFile> AdminFileRepository { get; init; } = new();
}
