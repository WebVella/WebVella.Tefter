﻿namespace WebVella.Tefter.Web.Models;

[JsonDerivedType(typeof(TucFilterBase), "fb")]
[JsonDerivedType(typeof(TucFilterAnd), "and")]
[JsonDerivedType(typeof(TucFilterOr), "or")]
[JsonDerivedType(typeof(TucFilterBoolean), "bool")]
[JsonDerivedType(typeof(TucFilterDateTime), "datetime")]
[JsonDerivedType(typeof(TucFilterGuid), "guid")]
[JsonDerivedType(typeof(TucFilterNumeric), "numeric")]
[JsonDerivedType(typeof(TucFilterText), "text")]
public record TucFilterBase
{
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();
	[JsonPropertyName("cn")]
	public string ColumnName { get; set; }

	[JsonPropertyName("v")]
	public string Value { get; set; } = null;

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
			return TucFilterOr.GetFilterType();
		}
		else if (model is TucFilterBoolean)
		{
			return TucFilterBoolean.GetFilterType();
		}
		else if (model is TucFilterDateTime)
		{
			return TucFilterDateTime.GetFilterType();
		}
		else if (model is TucFilterGuid)
		{
			return TucFilterGuid.GetFilterType();
		}
		else if (model is TucFilterNumeric)
		{
			return TucFilterNumeric.GetFilterType();
		}
		else if (model is TucFilterText)
		{
			return TucFilterText.GetFilterType();
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
		//else if (model is TfFilterDateOnly)
		//{
		//	return new TucFilterDateOnly(model);
		//}
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

	public static TucFilterBase FromQuery(TucFilterQuery model)
	{
		//rules
		if (model.Type == (new TucFilterAnd()).GetFilterType())
		{
			if (model.Name == (new TucFilterAnd()).GetColumnName())
			{
				return new TucFilterAnd(model);
			}
			else if (model.Name == (new TucFilterOr()).GetColumnName())
			{
				return new TucFilterOr(model);
			}
			throw new Exception("Unsupported RULE TucFilterQuery type for conversion to TucFilterBase");
		}
		else if (model.Type == TucFilterBoolean.GetFilterType())
		{
			return new TucFilterBoolean(model);
		}
		else if (model.Type == TucFilterDateTime.GetFilterType())
		{
			return new TucFilterDateTime(model);
		}
		else if (model.Type == TucFilterGuid.GetFilterType())
		{
			return new TucFilterGuid(model);
		}
		else if (model.Type == TucFilterNumeric.GetFilterType())
		{
			return new TucFilterNumeric(model);
		}
		else if (model.Type == TucFilterText.GetFilterType())
		{
			return new TucFilterText(model);
		}
		else throw new Exception("Unsupported TucFilterQuery type for conversion to TucFilterBase");
	}

	public static TucFilterQuery ToQuery(TucFilterBase model)
	{
		if (model is TucFilterAnd)
		{
			return ((TucFilterAnd)model).ToQuery();
		}
		else if (model is TucFilterOr)
		{
			return ((TucFilterOr)model).ToQuery();
		}
		else if (model is TucFilterBoolean)
		{
			return ((TucFilterBoolean)model).ToQuery();
		}
		else if (model is TucFilterDateTime)
		{
			return ((TucFilterDateTime)model).ToQuery();
		}
		else if (model is TucFilterGuid)
		{
			return ((TucFilterGuid)model).ToQuery();
		}
		else if (model is TucFilterNumeric)
		{
			return ((TucFilterNumeric)model).ToQuery();
		}
		else if (model is TucFilterText)
		{
			return ((TucFilterText)model).ToQuery();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterAnd");
	}

}

public record TucFilterAnd : TucFilterBase
{
	[JsonPropertyName("ft")]
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
				Filters.Add(FromModel(filter));
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

	public TucFilterAnd(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Filters = new();
		foreach (var item in model.Items)
		{
			Filters.Add(TucFilterBase.FromQuery(item));
		}
	}
	public TucFilterQuery ToQuery()
	{
		var query = new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName()
		};
		foreach (var item in Filters)
		{
			query.Items.Add(TucFilterBase.ToQuery(item));
		}
		return query;
	}
}

public record TucFilterOr : TucFilterBase
{
	[JsonPropertyName("ft")]
	public List<TucFilterBase> Filters { get; set; } = new();
	public string GetColumnName() => "OR";
	public static string GetFilterType() => "rule";

	public TucFilterOr() { }

	public TucFilterOr(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterOr)
		{
			var item = (TfFilterOr)model;
			foreach (var filter in item.Filters)
			{
				Filters.Add(FromModel(filter));
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

	public TucFilterOr(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Filters = new();
		foreach (var item in model.Items)
		{
			Filters.Add(TucFilterBase.FromQuery(item));
		}
	}
	public TucFilterQuery ToQuery()
	{
		var query = new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName()
		};
		foreach (var item in Filters)
		{
			query.Items.Add(TucFilterBase.ToQuery(item));
		}
		return query;
	}
}

public record TucFilterBoolean : TucFilterBase
{
	[JsonPropertyName("m")]
	public TucFilterBooleanComparisonMethod ComparisonMethod { get; set; } = TucFilterBooleanComparisonMethod.Equal;
	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "boolean";
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
	public Option<string> ValueOption
	{
		get => (Value is null ? ValueOptions.Single(x=> x.Value == "null") : ValueOptions.Single(x=> x.Value == Value));
	}

	public void ValueOptionChanged(Option<string> option)
	{
		if (option.Value == "null") Value = null;
		else if (Boolean.TryParse(option.Value, out bool outVal))
			Value = option.Value;
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
			if (!String.IsNullOrWhiteSpace(item.Value) && bool.TryParse(item.Value, out bool outVal))
				Value = item.Value;
			else
				Value = null;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterBooleanComparisonMethod, TucFilterBooleanComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterBoolean");
	}

	public TfFilterBoolean ToModel()
	{
		return new TfFilterBoolean(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterBooleanComparisonMethod, TfFilterBooleanComparisonMethod>(),
			value: Value?.ToString()
		);

	}

	public TucFilterBoolean(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && Boolean.TryParse(model.Value, out bool outVal)) Value = model.Value;
		ComparisonMethod = (TucFilterBooleanComparisonMethod)model.Method;
	}
	public TucFilterQuery ToQuery()
	{
		return new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}

public record TucFilterDateTime : TucFilterBase
{

	[JsonPropertyName("m")]
	public TucFilterDateTimeComparisonMethod ComparisonMethod { get; set; } = TucFilterDateTimeComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "datetime";
	public bool RequiresValue
	{
		get
		{
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasValue) return false;
			if (ComparisonMethod == TucFilterDateTimeComparisonMethod.HasNoValue) return false;
			return true;
		}
	}
	public string ValueProcessed
	{
		get => Value;
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
	public TucFilterDateTime() { }
	public TucFilterDateTime(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterDateTime)
		{
			var item = (TfFilterDateTime)model;
			if (String.IsNullOrWhiteSpace(item.Value)) Value = null;
			else if (FormulaUtility.GetDateFromFormulaString(item.Value) != null)
			{
				Value = item.Value;
			}
			else if (DateTime.TryParse(item.Value, out DateTime outVal2))
				Value = item.Value;
			else
				Value = null;

			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterDateTimeComparisonMethod, TucFilterDateTimeComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterDateTime");
	}
	public TfFilterDateTime ToModel()
	{
		return new TfFilterDateTime(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterDateTimeComparisonMethod, TfFilterDateTimeComparisonMethod>(),
			value: Value?.ToString()
		);
	}

	public TucFilterDateTime(TucFilterQuery model)
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
		ComparisonMethod = (TucFilterDateTimeComparisonMethod)model.Method;
	}
	public TucFilterQuery ToQuery()
	{
		return new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}

public record TucFilterGuid : TucFilterBase
{
	[JsonPropertyName("m")]
	public TucFilterGuidComparisonMethod ComparisonMethod { get; set; } = TucFilterGuidComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "guid";

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
	public string ValueProcessed
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
	public TucFilterGuid() { }

	public TucFilterGuid(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterGuid)
		{
			var item = (TfFilterGuid)model;
			if (!string.IsNullOrWhiteSpace(item.Value) && Guid.TryParse(item.Value, out Guid outVal))
				Value = item.Value;
			else
				Value = null;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterGuidComparisonMethod, TucFilterGuidComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterGuid");
	}

	public TfFilterGuid ToModel()
	{
		return new TfFilterGuid(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterGuidComparisonMethod, TfFilterGuidComparisonMethod>(),
			value: Value?.ToString()
		);
	}

	public TucFilterGuid(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && Guid.TryParse(model.Value, out Guid outVal)) Value = model.Value;
		ComparisonMethod = (TucFilterGuidComparisonMethod)model.Method;
	}
	public TucFilterQuery ToQuery()
	{
		return new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}

public record TucFilterNumeric : TucFilterBase
{
	[JsonPropertyName("m")]
	public TucFilterNumericComparisonMethod ComparisonMethod { get; set; } = TucFilterNumericComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "numeric";
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


	public TucFilterNumeric() { }

	public TucFilterNumeric(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterNumeric)
		{
			var item = (TfFilterNumeric)model;
			if (!string.IsNullOrWhiteSpace(item.Value) && decimal.TryParse(item.Value, out decimal outVal))
				Value = item.Value;
			else
				Value = null;
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterNumericComparisonMethod, TucFilterNumericComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterNumeric");
	}

	public TfFilterNumeric ToModel()
	{
		return new TfFilterNumeric(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterNumericComparisonMethod, TfFilterNumericComparisonMethod>(),
			value: Value?.ToString()
		);
	}

	public TucFilterNumeric(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = null;
		if (!String.IsNullOrWhiteSpace(model.Value) && decimal.TryParse(model.Value, out decimal outVal)) Value = model.Value;
		ComparisonMethod = (TucFilterNumericComparisonMethod)model.Method;
	}
	public TucFilterQuery ToQuery()
	{
		return new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value?.ToString(),
			Method = (int)ComparisonMethod,
		};
	}
}

public record TucFilterText : TucFilterBase
{
	[JsonPropertyName("m")]
	public TucFilterTextComparisonMethod ComparisonMethod { get; set; } = TucFilterTextComparisonMethod.Equal;

	public string GetColumnName() => ColumnName;
	public static string GetFilterType() => "text";

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
			ComparisonMethod = item.ComparisonMethod.ConvertSafeToEnum<TfFilterTextComparisonMethod, TucFilterTextComparisonMethod>();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterText");
	}

	public TfFilterText ToModel()
	{
		return new TfFilterText(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod.ConvertSafeToEnum<TucFilterTextComparisonMethod, TfFilterTextComparisonMethod>(),
			value: Value
		);
	}

	public TucFilterText(TucFilterQuery model)
	{
		Id = Guid.NewGuid();
		ColumnName = model.Name;
		Value = model.Value;
		ComparisonMethod = (TucFilterTextComparisonMethod)model.Method;
	}
	public TucFilterQuery ToQuery()
	{
		return new TucFilterQuery
		{
			Type = GetFilterType(),
			Name = GetColumnName(),
			Value = Value,
			Method = (int)ComparisonMethod,
		};
	}
}