namespace WebVella.Tefter;

public class TfFilterDateTime : TfFilterBase
{
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; init; }

	public DateTime? Value { get; init; }

	public TfFilterDateTime(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		DateTime? value)
		: base(columnName)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
