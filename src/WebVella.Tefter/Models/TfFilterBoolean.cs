namespace WebVella.Tefter;

public class TfFilterBoolean : TfFilterBase
{
	public TfFilterBooleanComparisonMethod ComparisonMethod { get; init; }

	public TfFilterBoolean(
		string columnName,
		TfFilterBooleanComparisonMethod comparisonMethod,
		string value)
		: base(columnName,value)
	{
		ComparisonMethod = comparisonMethod;
	}
}
