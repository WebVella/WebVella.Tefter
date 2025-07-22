namespace WebVella.Tefter;

public record TfFilterBoolean : TfFilterBase
{
	[JsonPropertyName("m")]
	public TfFilterBooleanComparisonMethod ComparisonMethod { get; set; }
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "boolean";

	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TfFilterBooleanComparisonMethod.IsTrue) return false;
			if (ComparisonMethod == TfFilterBooleanComparisonMethod.IsFalse) return false;
			if (ComparisonMethod == TfFilterBooleanComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TfFilterBooleanComparisonMethod.HasNoValue) return false;


			return true;
		}
	}

	[JsonIgnore]
	public IEnumerable<Option<string>> ValueOptions
	{
		get => new List<Option<string>>{
			new Option<string>{Value="true",Text="TRUE"},
			new Option<string>{Value="false",Text="FALSE"},
			new Option<string>{Value="null",Text="NULL"}
		};
	}
	[JsonIgnore]
	public Option<string> ValueOption
	{
		get => (Value is null ? ValueOptions.Single(x => x.Value == "null") : ValueOptions.Single(x => x.Value == Value));
	}

	public void ValueOptionChanged(Option<string> option)
	{
		if (option.Value == "null") Value = null;
		else if (Boolean.TryParse(option.Value, out bool outVal))
			Value = option.Value;
		else
			Value = null;
	}

	public TfFilterBoolean() : base(String.Empty,String.Empty) { }

	public TfFilterBoolean(
		string columnName,
		TfFilterBooleanComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterBoolean(TfFilterQuery model)
		: base(String.Empty, String.Empty)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && Boolean.TryParse(model.Value, out bool outVal)) Value = model.Value;
		ComparisonMethod = (TfFilterBooleanComparisonMethod)model.Method;
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
