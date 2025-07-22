namespace WebVella.Tefter;

public record TfFilterDateTime : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; set; }
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

	public void ValueStringChanged(string value)
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
	public TfFilterDateTime(
		string columnName,
		TfFilterDateTimeComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterDateTime(TfFilterQuery model) : base(String.Empty, null)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		if (String.IsNullOrWhiteSpace(model.Value)) Value = null;
		else if (FormulaUtility.GetDateFromFormulaString(model.Value) != null)
		{
			Value = model.Value;
		}
		else if (DateTime.TryParse(model.Value, out DateTime outVal2))
			Value = model.Value;
		else
			Value = null;
		ComparisonMethod = (TfFilterDateTimeComparisonMethod)model.Method;
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
