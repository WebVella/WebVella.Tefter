namespace WebVella.Tefter;

[JsonDerivedType(typeof(TfFilterBase), "fb")]
[JsonDerivedType(typeof(TfFilterBoolean), typeDiscriminator: "boolean")]
[JsonDerivedType(typeof(TfFilterDateTime), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(TfFilterGuid), typeDiscriminator: "guid")]
[JsonDerivedType(typeof(TfFilterNumeric), typeDiscriminator: "numeric")]
[JsonDerivedType(typeof(TfFilterOr), typeDiscriminator: "or")]
[JsonDerivedType(typeof(TfFilterAnd), typeDiscriminator: "and")]
[JsonDerivedType(typeof(TfFilterText), typeDiscriminator: "text")]
public abstract class TfFilterBase
{
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();

	[JsonPropertyName("cn")]
	public string ColumnName { get; set; }
	[JsonPropertyName("v")]
	public string? Value { get; init; }

	public TfFilterBase(
		string columnName,
		string? value)
	{
		ColumnName = columnName;
		Value = value;
	}

	public void FixPrefix(string prefix)
	{
		if (!ColumnName.StartsWith(prefix))
		{
			ColumnName = prefix + ColumnName;
		}

		if (this.GetType() == typeof(TfFilterAnd))
		{
			var column = (TfFilterAnd)this;
			foreach (var subFilter in column.Filters)
			{
				subFilter.FixPrefix(prefix);
			}
		}

		else if (this.GetType() == typeof(TfFilterOr))
		{
			var column = (TfFilterOr)this;
			foreach (var subFilter in column.Filters)
			{
				subFilter.FixPrefix(prefix);
			}
		}
	}

public static string GetColumnName(TfFilterBase model)
	{
		if (model is TfFilterAnd)
		{
			var item = (TfFilterAnd)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterOr)
		{
			var item = (TfFilterOr)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterBoolean)
		{
			var item = (TfFilterBoolean)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterDateTime)
		{
			var item = (TfFilterDateTime)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterGuid)
		{
			var item = (TfFilterGuid)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterNumeric)
		{
			var item = (TfFilterNumeric)model;
			return item.GetColumnName();
		}
		else if (model is TfFilterText)
		{
			var item = (TfFilterText)model;
			return item.GetColumnName();
		}
		else throw new Exception("Unsupported TfFilterBase in GetColumnName");
	}
	public static string GetFieldType(TfFilterBase model)
	{
		if (model is TfFilterAnd)
		{
			var item = (TfFilterAnd)model;
			return item.GetFilterType();
		}
		else if (model is TfFilterOr)
		{
			return TfFilterOr.GetFilterType();
		}
		else if (model is TfFilterBoolean)
		{
			return TfFilterBoolean.GetFilterType();
		}
		else if (model is TfFilterDateTime)
		{
			return TfFilterDateTime.GetFilterType();
		}
		else if (model is TfFilterGuid)
		{
			return TfFilterGuid.GetFilterType();
		}
		else if (model is TfFilterNumeric)
		{
			return TfFilterNumeric.GetFilterType();
		}
		else if (model is TfFilterText)
		{
			return TfFilterText.GetFilterType();
		}
		else throw new Exception("Unsupported TfFilterBase in GetColumnName");
	}
	public static ReadOnlyCollection<TfFilterBase> GetChildFilters(TfFilterBase model)
	{
		if (model is TfFilterAnd)
		{
			var item = (TfFilterAnd)model;
			return item.Filters;
		}
		else if (model is TfFilterOr)
		{
			var item = (TfFilterOr)model;
			return item.Filters;
		}
		else
		{
			return new List<TfFilterBase>().AsReadOnly();
		}
	}

public static TfFilterBase FromQuery(TfFilterQuery model)
	{
		//rules
		if (model.Type == (new TfFilterAnd()).GetFilterType())
		{
			if (model.Name == (new TfFilterAnd()).GetColumnName())
			{
				return new TfFilterAnd(model);
			}
			else if (model.Name == (new TfFilterOr()).GetColumnName())
			{
				return new TfFilterOr(model);
			}
			throw new Exception("Unsupported RULE TfFilterQuery type for conversion to TfFilterBase");
		}
		else if (model.Type == TfFilterBoolean.GetFilterType())
		{
			return new TfFilterBoolean(model);
		}
		else if (model.Type == TfFilterDateTime.GetFilterType())
		{
			return new TfFilterDateTime(model);
		}
		else if (model.Type == TfFilterGuid.GetFilterType())
		{
			return new TfFilterGuid(model);
		}
		else if (model.Type == TfFilterNumeric.GetFilterType())
		{
			return new TfFilterNumeric(model);
		}
		else if (model.Type == TfFilterText.GetFilterType())
		{
			return new TfFilterText(model);
		}
		else throw new Exception("Unsupported TfFilterQuery type for conversion to TfFilterBase");
	}

	public static TfFilterQuery ToQuery(TfFilterBase model)
	{
		if (model is TfFilterAnd)
		{
			return ((TfFilterAnd)model).ToQuery();
		}
		else if (model is TfFilterOr)
		{
			return ((TfFilterOr)model).ToQuery();
		}
		else if (model is TfFilterBoolean)
		{
			return ((TfFilterBoolean)model).ToQuery();
		}
		else if (model is TfFilterDateTime)
		{
			return ((TfFilterDateTime)model).ToQuery();
		}
		else if (model is TfFilterGuid)
		{
			return ((TfFilterGuid)model).ToQuery();
		}
		else if (model is TfFilterNumeric)
		{
			return ((TfFilterNumeric)model).ToQuery();
		}
		else if (model is TfFilterText)
		{
			return ((TfFilterText)model).ToQuery();
		}
		else throw new Exception("Unsupported TfFilterBase base type for conversion to TfFilterAnd");
	}
}
