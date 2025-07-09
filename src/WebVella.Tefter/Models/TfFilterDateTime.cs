namespace WebVella.Tefter;

public class TfFilterDateTime : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; init; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "datetime";
	public TfFilterDateTime(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterDateTime(TfFilterQuery model) : base(String.Empty, null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		if (String.IsNullOrWhiteSpace(model.Value)) Value = null;
		else if (FormulaUtility.GetDateFromFormulaString(model.Value) != null)
		{
			Value = model.Value;
		}
		else if (DateTime.TryParse(model.Value, out DateTime outVal2))
			Value = model.Value;
		else
			Value = null;
		ComparisonMethod = (TfFilterDateTimeComparisonMethod)model.Method;
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}
