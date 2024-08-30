namespace WebVella.Tefter;

[JsonDerivedType(typeof(TfFilterBoolean), typeDiscriminator: "boolean")]
[JsonDerivedType(typeof(TfFilterDateOnly), typeDiscriminator: "dateonly")]
[JsonDerivedType(typeof(TfFilterDateTime), typeDiscriminator: "datetime")]
[JsonDerivedType(typeof(TfFilterGuid), typeDiscriminator: "guid")]
[JsonDerivedType(typeof(TfFilterNumeric), typeDiscriminator: "numeric")]
[JsonDerivedType(typeof(TfFilterOr), typeDiscriminator: "or")]
[JsonDerivedType(typeof(TfFilterAnd), typeDiscriminator: "and")]
[JsonDerivedType(typeof(TfFilterText), typeDiscriminator: "text")]
public abstract class TfFilterBase
{
	public string ColumnName { get; init; }

	public TfFilterBase(string columnName)
	{
		ColumnName = columnName;
	}

}
