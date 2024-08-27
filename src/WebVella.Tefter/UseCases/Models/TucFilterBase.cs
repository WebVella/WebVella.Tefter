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
	public TfFilterBooleanComparisonMethod ComparisonMethod { get; set; }

	public TucFilterBoolean() { }

	public TucFilterBoolean(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterBoolean)
		{
			var item = (TfFilterBoolean)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterBoolean");
	}

	public TfFilterBoolean ToModel()
	{
		return new TfFilterBoolean(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);

	}
}

public record TucFilterDateOnly : TucFilterBase
{
	public DateOnly? Value { get; set; } = null;
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; set; }

	public TucFilterDateOnly() { }

	public TucFilterDateOnly(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterDateOnly)
		{
			var item = (TfFilterDateOnly)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterDateOnly");
	}

	public TfFilterDateOnly ToModel()
	{
		return new TfFilterDateOnly(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);
	}
}

public record TucFilterDateTime : TucFilterBase
{
	public DateTime? Value { get; set; } = null;
	public TfFilterDateTimeComparisonMethod ComparisonMethod { get; set; }

	public TucFilterDateTime() { }

	public TucFilterDateTime(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterDateTime)
		{
			var item = (TfFilterDateTime)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterDateTime");
	}

	public TfFilterDateTime ToModel()
	{
		return new TfFilterDateTime(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);
	}
}

public record TucFilterGuid : TucFilterBase
{
	public Guid? Value { get; set; } = null;
	public TfFilterGuidComparisonMethod ComparisonMethod { get; set; }

	public TucFilterGuid() { }

	public TucFilterGuid(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterGuid)
		{
			var item = (TfFilterGuid)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterGuid");
	}

	public TfFilterGuid ToModel()
	{
		return new TfFilterGuid(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);
	}
}

public record TucFilterNumeric : TucFilterBase
{
	public decimal? Value { get; set; } = null;
	public TfFilterNumericComparisonMethod ComparisonMethod { get; set; }

	public TucFilterNumeric() { }

	public TucFilterNumeric(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterNumeric)
		{
			var item = (TfFilterNumeric)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterNumeric");
	}

	public TfFilterNumeric ToModel()
	{
		return new TfFilterNumeric(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);
	}
}

public record TucFilterText : TucFilterBase
{
	public string Value { get; set; } = null;
	public TfFilterTextComparisonMethod ComparisonMethod { get; set; }

	public TucFilterText() { }

	public TucFilterText(TfFilterBase model)
	{
		ColumnName = model.ColumnName;

		if (model is TfFilterText)
		{
			var item = (TfFilterText)model;
			Value = item.Value;
			ComparisonMethod = item.ComparisonMethod;
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TucFilterText");
	}

	public TfFilterText ToModel()
	{
		return new TfFilterText(
			columnName: ColumnName,
			comparisonMethod: ComparisonMethod,
			value: Value
		);
	}
}