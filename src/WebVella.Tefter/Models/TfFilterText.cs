namespace WebVella.Tefter;

public record TfFilterText : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterTextComparisonMethod ComparisonMethod { get; set; } = TfFilterTextComparisonMethod.Equal;
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
	public void ValueChanged(string? value)
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
	public TfFilterText(TfFilterQuery model, string columnName) : base(string.Empty,null)
	{
		if (model is null) throw new ArgumentException("model is required",nameof(model));
		if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName is required",nameof(columnName));

		Id = Guid.NewGuid();
		Value = model.Value;
		ColumnName = columnName;
		ComparisonMethod = Utility.EnumExtensions.ConvertIntToEnum<TfFilterTextComparisonMethod>(model.Method,TfFilterTextComparisonMethod.Equal);
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Name = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}
