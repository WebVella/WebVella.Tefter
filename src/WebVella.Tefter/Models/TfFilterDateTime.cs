namespace WebVella.Tefter;

public class TfFilterDateTime : TfFilterBase
{
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; init; }

	public TfFilterDateTime(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}
}
