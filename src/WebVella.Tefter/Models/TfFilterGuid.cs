namespace WebVella.Tefter;

public record TfFilterGuid : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterGuidComparisonMethod ComparisonMethod { get; set; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "guid";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TfFilterGuidComparisonMethod.IsEmpty) return false;
			if (ComparisonMethod == TfFilterGuidComparisonMethod.IsNotEmpty) return false;
			if (ComparisonMethod == TfFilterGuidComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TfFilterGuidComparisonMethod.HasNoValue) return false;
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
		else if (Guid.TryParse(value, out Guid outVal))
			Value = value;
		else
			Value = null;
	}
	public TfFilterGuid() : base(String.Empty,String.Empty) { }
	public TfFilterGuid(
		string columnName,
		TfFilterGuidComparisonMethod comparisonMethod,
		string? value)
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
