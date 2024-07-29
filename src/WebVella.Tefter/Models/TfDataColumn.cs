namespace WebVella.Tefter;

public class TfDataColumn
{
	public TfDataTable DataTable { get; init; }
	public string Name { get; init; }
	public DatabaseColumnType DbType { get; init; }
	public bool IsNullable { get; init; }
	public bool IsShared { get; init; }
	public bool IsSystem { get; init; }

	internal TfDataColumn( 
		TfDataTable dataTable,
		string name,
		DatabaseColumnType dbType,
		bool isNullable,
		bool isShared,
		bool isSystem )
	{
		DataTable = dataTable;
		Name = name;
		DbType = dbType;
		IsNullable = isNullable;
		IsShared = isShared;
		IsSystem = isSystem;
	}

	public override string ToString()
	{
		return $"{Name}  [{DbType}]";
	}
}
