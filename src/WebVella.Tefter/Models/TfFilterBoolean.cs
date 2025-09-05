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

	public TfFilterBoolean() : base(String.Empty, String.Empty) { }

	public TfFilterBoolean(
		string columnName,
		TfFilterBooleanComparisonMethod comparisonMethod,
		string? value)
		: base(columnName, value)
	{
		ComparisonMethod = comparisonMethod;
	}

	public TfFilterBoolean(TfFilterQuery model, string columnName)
		: base(String.Empty, String.Empty)
	{
		if (model is null) throw new ArgumentException("model is required", nameof(model));
		if (String.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("columnName is required", nameof(columnName));

		Id = Guid.NewGuid();
		Value = null;
		ColumnName = columnName;
		if (!String.IsNullOrWhiteSpace(model.Value) && Boolean.TryParse(model.Value, out bool outVal)) Value = model.Value;
		ComparisonMethod = Utility.EnumExtensions.ConvertIntToEnum<TfFilterBooleanComparisonMethod>(model.Method,TfFilterBooleanComparisonMethod.IsTrue);
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
