namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnTypeAddon : ITfAddon
{
	public List<TfSpaceViewColumnAddonDataMapping> DataMapping { get; init; }
	
	//Setups Excel cell with value and formatting
	void ProcessExcelCell(
		TfSpaceViewColumnScreenRegionContext regionContext,
		IXLCell excelCell);
	//Returns Value/s as string usually for CSV export
	string? GetValueAsString(
		TfSpaceViewColumnScreenRegionContext regionContext);
	//Returns fragment to be rendered
	RenderFragment Render(TfSpaceViewColumnScreenRegionContext regionContext);
}

public record TfSpaceViewColumnAddonDataMapping
{
	public string Alias { get; init; }
	public string Description { get; init; }
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; }
}


public class TfSpaceViewColumnTypeAddonMeta
{
	public ITfSpaceViewColumnTypeAddon Instance { get; init; }
}
