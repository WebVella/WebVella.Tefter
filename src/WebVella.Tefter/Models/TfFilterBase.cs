namespace WebVella.Tefter;

[JsonDerivedType(typeof(TfFilterBoolean), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterDateOnly), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterDateTime), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterGuid), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterNumeric), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterOr), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterAnd), typeDiscriminator: "text")]
[JsonDerivedType(typeof(TfFilterText), typeDiscriminator: "text")]
public abstract class TfFilterBase
{
	public string ColumnName { get; init; }

	public TfFilterBase(string columnName)
	{
		ColumnName = columnName;
	}

}
