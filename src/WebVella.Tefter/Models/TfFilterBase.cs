namespace WebVella.Tefter;

[JsonDerivedType(typeof(TfFilterBoolean), typeDiscriminator: "boolean")]
[JsonDerivedType(typeof(TfFilterDateTime), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(TfFilterGuid), typeDiscriminator: "guid")]
[JsonDerivedType(typeof(TfFilterNumeric), typeDiscriminator: "numeric")]
[JsonDerivedType(typeof(TfFilterOr), typeDiscriminator: "or")]
[JsonDerivedType(typeof(TfFilterAnd), typeDiscriminator: "and")]
[JsonDerivedType(typeof(TfFilterText), typeDiscriminator: "text")]
public abstract class TfFilterBase
{
	public string ColumnName { get; set; }
	public string Value { get; init; }

	public TfFilterBase(
		string columnName,
		string value)
	{
		ColumnName = columnName;
		Value = value;
	}

	public void FixProviderPrefix(string dpPrefix)
	{
		if (!ColumnName.StartsWith(dpPrefix))
		{
			ColumnName = dpPrefix + ColumnName;
		}

		if (this.GetType() == typeof(TfFilterAnd))
		{
			var column = (TfFilterAnd)this;
			foreach (var subFilter in column.Filters)
			{
				subFilter.FixProviderPrefix(dpPrefix);
			}
		}

		else if (this.GetType() == typeof(TfFilterOr))
		{
			var column = (TfFilterOr)this;
			foreach (var subFilter in column.Filters)
			{
				subFilter.FixProviderPrefix(dpPrefix);
			}
		}
	}

}
