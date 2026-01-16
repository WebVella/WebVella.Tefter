namespace WebVella.Tefter;

public record TfFilterNumeric : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterNumericComparisonMethod ComparisonMethod { get; set; } = TfFilterNumericComparisonMethod.Equal;
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
	
	public void ValueStringChanged(string? value)
	{
		if (String.IsNullOrWhiteSpace(value)) Value = null;
		else if (decimal.TryParse(value, out decimal outVal))
			Value = value;
		else
			Value = null;
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
	public TfFilterNumeric(int comparisonMethodInt) : base(String.Empty, String.Empty)
	{
		ComparisonMethod = comparisonMethodInt.ConvertIntToEnum<TfFilterNumericComparisonMethod>(TfFilterNumericComparisonMethod.Equal);
	}
	public TfFilterNumeric(
		string columnName,
		TfFilterNumericComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}
	public TfFilterNumeric(TfFilterQuery model, string columnName) : base(String.Empty,null)
	{
		if (model is null) throw new ArgumentException("model is required",nameof(model));
		if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName is required",nameof(columnName));

		Id = Guid.NewGuid();
		Value = null;
		ColumnName = columnName;
		if (!String.IsNullOrWhiteSpace(model.Value) && decimal.TryParse(model.Value, out decimal outVal)) Value = model.Value;
		ComparisonMethod = Utility.EnumExtensions.ConvertIntToEnum<TfFilterNumericComparisonMethod>(model.Method,TfFilterNumericComparisonMethod.Equal);
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			QueryName = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}
