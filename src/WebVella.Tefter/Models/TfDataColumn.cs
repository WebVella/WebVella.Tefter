namespace WebVella.Tefter;

public class TfDataColumn
{
	public TfDataTable DataTable { get; init; }
	public string Name { get; init; }
	public TfDatabaseColumnType DbType { get; init; }
	public bool IsNullable { get; init; }	
	public TfDataColumnOriginType Origin { get; init; } = TfDataColumnOriginType.CurrentProvider;
	public bool IsReadOnly { get; init; } = false;

	internal TfDataColumn(
		TfDataTable dataTable,
		string name,
		TfDatabaseColumnType dbType,
		bool isNullable,
		TfDataColumnOriginType origin,
		bool isReadOnly)
	{
		DataTable = dataTable;
		Name = name;
		DbType = dbType;
		IsNullable = isNullable;
		Origin = origin;
		IsReadOnly = isReadOnly;
	}

	public override string ToString()
	{
		return $"{Name}  [{DbType}]";
	}
}