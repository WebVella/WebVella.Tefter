namespace WebVella.Tefter;

public record TfFilterText : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterTextComparisonMethod ComparisonMethod { get; set; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "text";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TfFilterTextComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TfFilterTextComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public void ValueChanged(string value)
	{
		Value = value;
	}
	public TfFilterText() : base(String.Empty,String.Empty) { }
	public TfFilterText(
		string columnName,
		TfFilterTextComparisonMethod comparisonMethod,
		string? value)
		: base(columnName,  value)
	{
		ComparisonMethod = comparisonMethod;
	}
	public TfFilterText(TfFilterQuery model) : base(string.Empty,null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = model.Value;
		ComparisonMethod = (TfFilterTextComparisonMethod)model.Method;
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}
