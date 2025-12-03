namespace WebVella.Tefter.Addons;

public interface ITfSpaceViewColumnTypeAddon : ITfAddon
{
	public List<TfSpaceViewColumnDataMappingDefinition> DataMappingDefinitions { get; init; }
	
	//Setups Excel cell with value and formatting
	void ProcessExcelCell(TfSpaceViewColumnBase args);
	//Returns Value/s as string usually for CSV export
	string GetValueAsString(TfSpaceViewColumnBase args);
	//Returns fragment to be rendered
	RenderFragment Render(TfSpaceViewColumnBase args);
	List<ValidationError> ValidateTypeOptions(TfSpaceViewColumnOptionsMode args);
}

public record TfSpaceViewColumnDataMappingDefinition
{
	public string Alias { get; init; } = null!;
	public string? Description { get; init; }

	public bool IsHidden { get; init; } = false;
	public List<TfDatabaseColumnType> SupportedDatabaseColumnTypes { get; init; } = new();
}
