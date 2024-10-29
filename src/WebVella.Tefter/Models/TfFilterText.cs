namespace WebVella.Tefter;

public class TfFilterText : TfFilterBase
{
	public TfFilterTextComparisonMethod ComparisonMethod { get; init; }

	public TfFilterText(
		string columnName,
		TfFilterTextComparisonMethod comparisonMethod,
		string value)
		: base(columnName,  value)
	{
		ComparisonMethod = comparisonMethod;
	}
}
