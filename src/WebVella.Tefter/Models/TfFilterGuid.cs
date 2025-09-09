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

	public TfFilterGuid(TfFilterQuery model, string columnName) : base(String.Empty,null)
	{
		if (model is null) throw new ArgumentException("model is required",nameof(model));
		if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName is required",nameof(columnName));

		Id = Guid.NewGuid();
		Value = null;
		ColumnName = columnName;
		if (!String.IsNullOrWhiteSpace(model.Value) && Guid.TryParse(model.Value, out Guid outVal)) Value = model.Value;
		ComparisonMethod = Utility.EnumExtensions.ConvertIntToEnum<TfFilterGuidComparisonMethod>(model.Method,TfFilterGuidComparisonMethod.Equal);
	}
	public TfFilterQuery ToQuery()
	{
		return new TfFilterQuery
		{
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}
