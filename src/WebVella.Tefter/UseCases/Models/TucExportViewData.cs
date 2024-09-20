namespace WebVella.Tefter.UseCases.Models;

public record TucExportViewData
{
	public TfRouteState RouteState { get; init; }
	public List<Guid> SelectedRows { get; init; } = new();//null or List.Empty means all rows


}
