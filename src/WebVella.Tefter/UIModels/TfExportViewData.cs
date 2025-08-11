using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfExportViewData
{
	public TfNavigationState RouteState { get; init; }
	public List<Guid> SelectedRows { get; init; } = new();//null or List.Empty means all rows
}
