namespace WebVella.Tefter;

public class TfFilterNumeric : TfFilterBase
{
	public TfFilterNumericComparisonMethod ComparisonMethod { get; init; }

	public TfFilterNumeric(
		string columnName, 
		TfFilterNumericComparisonMethod comparisonMethod,
		string value) 
		: base(columnName, value )
	{
		ComparisonMethod = comparisonMethod;
	}
}
