namespace WebVella.Tefter;

public record TfFilterDateTime : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; set; } = TfFilterDateTimeComparisonMethod.Greater;
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "datetime";

	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TfFilterDateTimeComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TfFilterDateTimeComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public string? ValueProcessed
	{
		get => Value?.ToString();
	}

	public void ValueStringChanged(string? value)
	{
		if (String.IsNullOrWhiteSpace(value)) Value = null;
		else if (FormulaUtility.GetDateFromFormulaString(value) != null)
		{
			Value = value;
		}
		else if (DateTime.TryParse(value, out DateTime outVal))
			Value = value;
		else
			Value = null;
	}
	public TfFilterDateTime() : base(String.Empty,String.Empty) { }
	public TfFilterDateTime(int comparisonMethodInt) : base(String.Empty, String.Empty)
	{
		ComparisonMethod = comparisonMethodInt.ConvertIntToEnum<TfFilterDateTimeComparisonMethod>(TfFilterDateTimeComparisonMethod.Greater);
	}
	public TfFilterDateTime(int comparisonMethodInt, string? value) : base(String.Empty, value)
	{
		ComparisonMethod = comparisonMethodInt.ConvertIntToEnum<TfFilterDateTimeComparisonMethod>(TfFilterDateTimeComparisonMethod.Greater);
	}
	public TfFilterDateTime(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterDateTime(TfFilterQuery model, string columnName) : base(String.Empty, null)
	{
		if (model is null) throw new ArgumentException("model is required",nameof(model));
		if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName is required",nameof(columnName));

		Id = Guid.NewGuid();
		Value = null;
		ColumnName = columnName;

		if (String.IsNullOrWhiteSpace(model.Value)) Value = null;
		else if (FormulaUtility.GetDateFromFormulaString(model.Value) != null)
		{
			Value = model.Value;
		}
		else if (DateTime.TryParse(model.Value, out DateTime outVal2))
			Value = model.Value;
		else
			Value = null;
		ComparisonMethod = Utility.EnumExtensions.ConvertIntToEnum<TfFilterDateTimeComparisonMethod>(model.Method,TfFilterDateTimeComparisonMethod.Greater);
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			QueryName = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}
