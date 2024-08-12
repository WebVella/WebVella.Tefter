namespace WebVella.Tefter;

public class TfFilterGuid : TfFilterBase
{
	public TfFilterGuidComparisonMethod ComparisonMethod { get; init; }

	public Guid? Value { get; init; }

	public TfFilterGuid(
		string columnName,
		TfFilterGuidComparisonMethod comparisonMethod,
		Guid? value)
		: base(columnName)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}
}
