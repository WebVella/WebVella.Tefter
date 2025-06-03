namespace WebVella.Tefter;

public class TfDataColumn
{
	public TfDataTable DataTable { get; init; }
	public string Name { get; init; }
	public TfDatabaseColumnType DbType { get; init; }
	public bool IsNullable { get; init; }
	public bool IsShared { get; init; }
	public bool IsSystem { get; init; }
	public bool IsJoinColumn { get; init; }
	public bool IsIdentityColumn { get; init; }

	internal TfDataColumn( 
		TfDataTable dataTable,
		string name,
		TfDatabaseColumnType dbType,
		bool isNullable,
		bool isShared,
		bool isSystem, 
		bool isJoinColumn,
		bool isIdentityColumn)
	{
		DataTable = dataTable;
		Name = name;
		DbType = dbType;
		IsNullable = isNullable;
		IsShared = isShared;
		IsSystem = isSystem;
		IsJoinColumn = isJoinColumn;
		IsIdentityColumn = isIdentityColumn;
	}

	public override string ToString()
	{
		return $"{Name}  [{DbType}]";
	}
}
