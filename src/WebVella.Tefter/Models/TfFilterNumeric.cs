namespace WebVella.Tefter;

public class TfFilterNumeric : TfFilterBase
{
	public TfFilterNumericComparisonMethod ComparisonMethod { get; init; }

	public decimal? Value { get; init; }

	public TfFilterNumeric(
		string columnName, 
		TfFilterNumericComparisonMethod comparisonMethod,
		decimal? value) 
		: base(columnName)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
