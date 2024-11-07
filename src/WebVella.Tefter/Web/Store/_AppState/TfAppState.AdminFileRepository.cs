namespace WebVella.Tefter.Web.Store;

public partial record TfAppState
{
	public List<TucRepositoryFile> AdminFileRepository { get; init; } = new();
}
