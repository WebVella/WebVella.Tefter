namespace WebVella.Tefter;

public class TfFilterNumeric : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterNumericComparisonMethod ComparisonMethod { get; init; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "numeric";
	public TfFilterNumeric(
		string columnName,
		TfFilterNumericComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}
	public TfFilterNumeric(TfFilterQuery model) : base(String.Empty,null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && decimal.TryParse(model.Value, out decimal outVal)) Value = model.Value;
		ComparisonMethod = (TfFilterNumericComparisonMethod)model.Method;
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
