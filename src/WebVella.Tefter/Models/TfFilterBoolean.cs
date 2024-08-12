namespace WebVella.Tefter;

public class TfFilterBoolean : TfFilterBase
{
	public TfFilterBooleanComparisonMethod ComparisonMethod { get; init; }

	public bool? Value { get; init; }

	public TfFilterBoolean(
		string columnName,
		TfFilterBooleanComparisonMethod comparisonMethod,
		bool? value)
		: base(columnName)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
