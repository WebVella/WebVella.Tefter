namespace WebVella.Tefter;

public class TfFilterGuid : TfFilterBase
{
	public TfFilterGuidComparisonMethod ComparisonMethod { get; init; }

	public TfFilterGuid(
		string columnName,
		TfFilterGuidComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
