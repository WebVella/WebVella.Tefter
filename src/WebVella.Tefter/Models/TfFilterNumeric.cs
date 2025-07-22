namespace WebVella.Tefter;

public record TfFilterNumeric : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterNumericComparisonMethod ComparisonMethod { get; set; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "numeric";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TfFilterNumericComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TfFilterNumericComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public void ValueChanged(decimal? value)
	{
		Value = value?.ToString(TfConstants.TF_FILTER_CULTURE);
	}
	public decimal? ValueAsDecimal
	{
		get
		{
			if (String.IsNullOrWhiteSpace(Value)) return null;

			decimal? result = null;
			if (decimal.TryParse(Value, TfConstants.TF_FILTER_CULTURE, out decimal outDec))
			{
				result = outDec;
			}
			return result;
		}
	}
	public TfFilterNumeric() : base(String.Empty,String.Empty) { }
	public TfFilterNumeric(
		string columnName,
		TfFilterNumericComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}
	public TfFilterNumeric(TfFilterQuery model) : base(String.Empty,null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && decimal.TryParse(model.Value, out decimal outVal)) Value = model.Value;
		ComparisonMethod = (TfFilterNumericComparisonMethod)model.Method;
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
