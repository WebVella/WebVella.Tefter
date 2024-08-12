namespace WebVella.Tefter;

public class TfFilterDateOnly : TfFilterBase
{
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; init; }

	public DateOnly? Value { get; init; }

	public TfFilterDateOnly(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		DateOnly? value)
		: base(columnName)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
