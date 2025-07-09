namespace WebVella.Tefter;

public class TfFilterGuid : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterGuidComparisonMethod ComparisonMethod { get; init; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "guid";

	public TfFilterGuid(
		string columnName,
		TfFilterGuidComparisonMethod comparisonMethod,
		string value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
		Value = value;
	}

	public TfFilterGuid(TfFilterQuery model) : base(String.Empty,null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && Guid.TryParse(model.Value, out Guid outVal)) Value = model.Value;
		ComparisonMethod = (TfFilterGuidComparisonMethod)model.Method;
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}
