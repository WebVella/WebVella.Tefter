namespace WebVella.Tefter;

[JsonDerivedType(typeof(TfFilterBase), "fb")]
[JsonDerivedType(typeof(TfFilterAnd), "and")]
[JsonDerivedType(typeof(TfFilterOr), "or")]
[JsonDerivedType(typeof(TfFilterBoolean), "bool")]
[JsonDerivedType(typeof(TfFilterDateTime), "datetime")]
[JsonDerivedType(typeof(TfFilterGuid), "guid")]
[JsonDerivedType(typeof(TfFilterNumeric), "numeric")]
[JsonDerivedType(typeof(TfFilterText), "text")]
public record TfFilterBase
{
	[JsonIgnore]
	public Guid Id { get; set; } = Guid.NewGuid();

	[JsonPropertyName("cn")]
	public string ColumnName { get; set; }
	[JsonPropertyName("v")]
	public string? Value { get; set; }
	public TfFilterBase() { }
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

	public string GetColumnName(TfFilterBase model)
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
	public string GetFieldType(TfFilterBase model)
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
	public ReadOnlyCollection<TfFilterBase> GetChildFilters(TfFilterBase model)
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

	public TfFilterBase? FromQuery(TfFilterQuery model, List<TfSpaceViewColumn> viewColumns, Dictionary<string, TfDatabaseColumnType> queryNameTypeDict)
	{
		if (model is null) throw new ArgumentException("required", nameof(model));
		if (viewColumns is null) throw new ArgumentException("required", nameof(viewColumns));
		if (queryNameTypeDict is null) throw new ArgumentException("required", nameof(queryNameTypeDict));

		//rules
		if (model.Name == (new TfFilterAnd()).GetColumnName())
		{
			return new TfFilterAnd(model, viewColumns, queryNameTypeDict);
		}
		else if (model.Name == (new TfFilterOr()).GetColumnName())
		{
			return new TfFilterOr(model, viewColumns, queryNameTypeDict);
		}
		else
		{
			var viewColumn = viewColumns.FirstOrDefault(x => x.QueryName.ToLowerInvariant() == model.Name.ToLowerInvariant());
			if (viewColumn is null) return null;
			var columnName = viewColumn.GetColumnNameFromDataMapping();
			if (String.IsNullOrWhiteSpace(columnName)) return null;
			if(!queryNameTypeDict.ContainsKey(model.Name)) return null;
			switch (queryNameTypeDict[model.Name])
			{
				case TfDatabaseColumnType.ShortInteger:
				case TfDatabaseColumnType.Integer:
				case TfDatabaseColumnType.LongInteger:
				case TfDatabaseColumnType.Number:
					return new TfFilterNumeric(model, columnName);
				case TfDatabaseColumnType.Boolean:
					return new TfFilterBoolean(model, columnName);
				case TfDatabaseColumnType.DateTime:
				case TfDatabaseColumnType.DateOnly:
					return new TfFilterDateTime(model, columnName);
				case TfDatabaseColumnType.ShortText:
				case TfDatabaseColumnType.Text:
					return new TfFilterText(model, columnName);
				case TfDatabaseColumnType.Guid:
					return new TfFilterGuid(model, columnName);
				default:
					throw new Exception("Unsupported TfFilterQuery type for conversion to TfFilterBase");
			}
		}
	}

	public TfFilterQuery ToQuery(TfFilterBase model)
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
