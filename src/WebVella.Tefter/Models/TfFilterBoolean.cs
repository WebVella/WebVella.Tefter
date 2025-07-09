namespace WebVella.Tefter;

public class TfFilterBoolean : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterBooleanComparisonMethod ComparisonMethod { get; init; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "boolean";
	public TfFilterBoolean(
		string columnName,
		TfFilterBooleanComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterBoolean(TfFilterQuery model)
		: base(String.Empty, String.Empty)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && Boolean.TryParse(model.Value, out bool outVal)) Value = model.Value;
		ComparisonMethod = (TfFilterBooleanComparisonMethod)model.Method;
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}
