namespace WebVella.Tefter;

public class TfDataColumn
{
	public TfDataTable DataTable { get; init; }
	public string Name { get; init; }
	public TfDatabaseColumnType DbType { get; init; }
	public bool IsNullable { get; init; }	
	public TfDataColumnOriginType OriginType { get; init; } = TfDataColumnOriginType.CurrentProvider;

	internal TfDataColumn(
		TfDataTable dataTable,
		string name,
		TfDatabaseColumnType dbType,
		bool isNullable,
		TfDataColumnOriginType originType )
	{
		DataTable = dataTable;
		Name = name;
		DbType = dbType;
		IsNullable = isNullable;
		OriginType = originType;
	}

	public override string ToString()
	{
		return $"{Name}  [{DbType}]";
	}
}