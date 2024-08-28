using WebVella.Tefter.Web.Utils;

namespace WebVella.Tefter.UseCases.Models;

[JsonDerivedType(typeof(TucFilterBase), typeDiscriminator: "base")]
[JsonDerivedType(typeof(TucFilterAnd), typeDiscriminator: "and")]
[JsonDerivedType(typeof(TucFilterOr), typeDiscriminator: "or")]
[JsonDerivedType(typeof(TucFilterBoolean), typeDiscriminator: "boolean")]
[JsonDerivedType(typeof(TucFilterDateOnly), typeDiscriminator: "dateonly")]
[JsonDerivedType(typeof(TucFilterDateTime), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(TucFilterGuid), typeDiscriminator: "guid")]
[JsonDerivedType(typeof(TucFilterNumeric), typeDiscriminator: "numeric")]
[JsonDerivedType(typeof(TucFilterText), typeDiscriminator: "text")]
public record TucFilterBase
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string ColumnName { get; set; }
	public static string GetColumnName(TucFilterBase model)
	{
		if (model is TucFilterAnd)
		{
			var item = (TucFilterAnd)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterOr)
		{
			var item = (TucFilterOr)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterBoolean)
		{
			var item = (TucFilterBoolean)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterDateOnly)
		{
			var item = (TucFilterDateOnly)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterDateTime)
		{
			var item = (TucFilterDateTime)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterGuid)
		{
			var item = (TucFilterGuid)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterNumeric)
		{
			var item = (TucFilterNumeric)model;
			return item.GetColumnName();
		}
		else if (model is TucFilterText)
		{
			var item = (TucFilterText)model;
			return item.GetColumnName();
		}
		else throw new Exception("Unsupported TucFilterBase in GetColumnName");
	}
	public static string GetFieldType(TucFilterBase model)
	{
		if (model is TucFilterAnd)
		{
			var item = (TucFilterAnd)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterOr)
		{
			var item = (TucFilterOr)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterBoolean)
		{
			var item = (TucFilterBoolean)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterDateOnly)
		{
			var item = (TucFilterDateOnly)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterDateTime)
		{
			var item = (TucFilterDateTime)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterGuid)
		{
			var item = (TucFilterGuid)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterNumeric)
		{
			var item = (TucFilterNumeric)model;
			return item.GetFilterType();
		}
		else if (model is TucFilterText)
		{
			var item = (TucFilterText)model;
			return item.GetFilterType();
		}
		else throw new Exception("Unsupported TucFilterBase in GetColumnName");
	}
	public static List<TucFilterBase> GetChildFilters(TucFilterBase model)
	{
		if (model is TucFilterAnd)
		{
			var item = (TucFilterAnd)model;
			return item.Filters;
		}
		else if (model is TucFilterOr)
		{
			var item = (TucFilterOr)model;
			return item.Filters;
		}
		else
		{
			return new List<TucFilterBase>();
		}
	}
	public static TucFilterBase FromModel(TfFilterBase model)
	{
		if (model is TfFilterAnd)
		{
			return new TucFilterAnd(model);
		}
		else if (model is TfFilterOr)
		{
			return new TucFilterOr(model);
		}
		else if (model is TfFilterBoolean)
		{
			return new TucFilterBoolean(model);
		}
		else if (model is TfFilterDateOnly)
		{
			return new TucFilterDateOnly(model);
		}
		else if (model is TfFilterDateTime)
		{
			return new TucFilterDateTime(model);
		}
		else if (model is TfFilterGuid)
		{
			return new TucFilterGuid(model);
		}
		else if (model is TfFilterNumeric)
		{
			return new TucFilterNumeric(model);
		}
		else if (model is TfFilterText)
		{
			return new TucFilterText(model);
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterAnd");
	}
	public static TfFilterBase ToModel(TucFilterBase model)
	{
		if (model is TucFilterAnd)
		{
			return ((TucFilterAnd)model).ToModel();
		}
		else if (model is TucFilterOr)
		{
			return ((TucFilterOr)model).ToModel();
		}
		else if (model is TucFilterBoolean)
		{
			return ((TucFilterBoolean)model).ToModel();
		}
		else if (model is TucFilterDateOnly)
		{
			return ((TucFilterDateOnly)model).ToModel();
		}
		else if (model is TucFilterDateTime)
		{
			return ((TucFilterDateTime)model).ToModel();
		}
		else if (model is TucFilterGuid)
		{
			return ((TucFilterGuid)model).ToModel();
		}
		else if (model is TucFilterNumeric)
		{
			return ((TucFilterNumeric)model).ToModel();
		}
		else if (model is TucFilterText)
		{
			return ((TucFilterText)model).ToModel();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterAnd");
	}
}

public record TucFilterAnd : TucFilterBase
{
	public List<TucFilterBase> Filters { get; set; } = new();
	public string GetColumnName() => "AND";
	public string GetFilterType() => "rule";

	public TucFilterAnd() { }

	public TucFilterAnd(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterAnd)
		{
			var item = (TfFilterAnd)model;
			foreach (var filter in item.Filters)
			{
				Filters.Add(FromModel(model));
			}
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterAnd");
	}

	public TfFilterAnd ToModel()
	{
		var model = new TfFilterAnd
		{
			ColumnName = ColumnName
		};
		foreach (var filter in Filters)
		{
			model.Add(ToModel(filter));
		}
		return model;
	}
}

public record TucFilterOr : TucFilterBase
{
	public List<TucFilterBase> Filters { get; set; } = new();
	public string GetColumnName() => "OR";
	public string GetFilterType() => "rule";

	public TucFilterOr() { }

	public TucFilterOr(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterOr)
		{
			var item = (TfFilterOr)model;
			foreach (var filter in item.Filters)
			{
				Filters.Add(FromModel(model));
			}
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterOr");
	}

	public TfFilterOr ToModel()
	{
		var model = new TfFilterOr
		{
			ColumnName = ColumnName
		};
		foreach (var filter in Filters)
		{
			model.Add(ToModel(filter));
		}
		return model;
	}
}

public record TucFilterBoolean : TucFilterBase
{
	public bool? Value { get; set; } = null;
	public TucFilterBooleanComparisonMethod ComparisonMethod { get; set; } = TucFilterBooleanComparisonMethod.Equal;
	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "boolean";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterBooleanComparisonMethod.IsTrue) return false;
			if (ComparisonMethod == TucFilterBooleanComparisonMethod.IsFalse) return false;
			if (ComparisonMethod == TucFilterBooleanComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterBooleanComparisonMethod.HasNoValue) return false;


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
	public string ValueString
	{
		get => (Value is null ? "null" : Value.Value.ToString()).ToLowerInvariant();
	}

	public void ValueOptionChanged(Option<string> option)
	{
		if (option.Value == "null") Value = null;
		else if (Boolean.TryParse(option.Value, out bool outVal))
			Value = outVal;
		else
			Value = null;
	}

	public TucFilterBoolean() { }

	public TucFilterBoolean(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterBoolean)
		{
			var item = (TfFilterBoolean)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterBooleanComparisonMethod,TucFilterBooleanComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterBoolean");
	}

	public TfFilterBoolean ToModel()
	{
		return new TfFilterBoolean(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterBooleanComparisonMethod,TfFilterBooleanComparisonMethod>(),
			value: Value
		);

	}
}

public record TucFilterDateOnly : TucFilterBase
{
	public DateOnly? Value { get; set; } = null;
	public TucFilterDateTimeComparisonMethod ComparisonMethod { get; set; } = TucFilterDateTimeComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "dateonly";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public string ValueString
	{
		get => Value?.ToString(TfConstants.DateOnlyFormatInput);
	}
	public void ValueStringChanged(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) Value = null;
		else if (DateOnly.TryParse(value, out DateOnly outVal))
			Value = outVal;
		else
			Value = null;
	}


	public TucFilterDateOnly() { }

	public TucFilterDateOnly(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterDateOnly)
		{
			var item = (TfFilterDateOnly)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterDateTimeComparisonMethod,TucFilterDateTimeComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterDateOnly");
	}

	public TfFilterDateOnly ToModel()
	{
		return new TfFilterDateOnly(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterDateTimeComparisonMethod,TfFilterDateTimeComparisonMethod>(),
			value: Value
		);
	}
}

public record TucFilterDateTime : TucFilterBase
{
	public DateTime? Value { get; set; } = null;
	public TucFilterDateTimeComparisonMethod ComparisonMethod { get; set; } = TucFilterDateTimeComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "datetime";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public string ValueString
	{
		get => Value?.ToString(TfConstants.DateTimeFormatInput);
	}
	public void ValueStringChanged(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) Value = null;
		else if (DateTime.TryParse(value, out DateTime outVal))
			Value = outVal;
		else
			Value = null;
	}
	public TucFilterDateTime() { }

	public TucFilterDateTime(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterDateTime)
		{
			var item = (TfFilterDateTime)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterDateTimeComparisonMethod,TucFilterDateTimeComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterDateTime");
	}

	public TfFilterDateTime ToModel()
	{
		return new TfFilterDateTime(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterDateTimeComparisonMethod,TfFilterDateTimeComparisonMethod>(),
			value: Value
		);
	}
}

public record TucFilterGuid : TucFilterBase
{
	public Guid? Value { get; set; } = null;
	public TucFilterGuidComparisonMethod ComparisonMethod { get; set; } = TucFilterGuidComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "guid";

	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterGuidComparisonMethod.IsEmpty) return false;
			if (ComparisonMethod == TucFilterGuidComparisonMethod.IsNotEmpty) return false;
			if (ComparisonMethod == TucFilterGuidComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterGuidComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public string ValueString
	{
		get => Value?.ToString();
	}
	public void ValueStringChanged(string value)
	{
		if (String.IsNullOrWhiteSpace(value)) Value = null;
		else if (Guid.TryParse(value, out Guid outVal))
			Value = outVal;
		else
			Value = null;
	}
	public TucFilterGuid() { }

	public TucFilterGuid(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterGuid)
		{
			var item = (TfFilterGuid)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterGuidComparisonMethod,TucFilterGuidComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterGuid");
	}

	public TfFilterGuid ToModel()
	{
		return new TfFilterGuid(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterGuidComparisonMethod,TfFilterGuidComparisonMethod>(),
			value: Value
		);
	}
}

public record TucFilterNumeric : TucFilterBase
{
	public decimal? Value { get; set; } = null;
	public TucFilterNumericComparisonMethod ComparisonMethod { get; set; } = TucFilterNumericComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "numeric";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterNumericComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterNumericComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public void ValueChanged(decimal? value)
	{
		Value = value;
	}

	public TucFilterNumeric() { }

	public TucFilterNumeric(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterNumeric)
		{
			var item = (TfFilterNumeric)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterNumericComparisonMethod,TucFilterNumericComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterNumeric");
	}

	public TfFilterNumeric ToModel()
	{
		return new TfFilterNumeric(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterNumericComparisonMethod,TfFilterNumericComparisonMethod>(),
			value: Value
		);
	}
}

public record TucFilterText : TucFilterBase
{
	public string Value { get; set; } = null;
	public TucFilterTextComparisonMethod ComparisonMethod { get; set; } = TucFilterTextComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public string GetFilterType() => "text";

	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterTextComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterTextComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public void ValueChanged(string value)
	{
		Value = value;
	}
	public TucFilterText() { }

	public TucFilterText(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterText)
		{
			var item = (TfFilterText)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterTextComparisonMethod,TucFilterTextComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterText");
	}

	public TfFilterText ToModel()
	{
		return new TfFilterText(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterTextComparisonMethod,TfFilterTextComparisonMethod>(),
			value: Value
		);
	}
}