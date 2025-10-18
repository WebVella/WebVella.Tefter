namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnTypeAddon : ITfAddon
{
	public List<TfSpaceViewColumnDataMappingDefinition> DataMappingDefinitions { get; init; }
	
	//Setups Excel cell with value and formatting
	void ProcessExcelCell(TfSpaceViewColumnBaseContext args);
	//Returns Value/s as string usually for CSV export
	string GetValueAsString(TfSpaceViewColumnBaseContext args);
	//Returns fragment to be rendered
	RenderFragment Render(TfSpaceViewColumnBaseContext args);
	List<ValidationError> ValidateTypeOptions(TfSpaceViewColumnOptionsModeContext args);
}

public record TfSpaceViewColumnDataMappingDefinition
{
	public string Alias { get; init; } = null!;
	public string? Description { get; init; }
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; } = new();
}
