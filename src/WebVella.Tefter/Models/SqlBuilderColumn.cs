namespace WebVella.Tefter.Models;
internal record SqlBuilderColumn
{
	public Guid Id { get; set; }
	public string DbName { get; set; }
	public string DataIdentity { get; set; }
	public TfDatabaseColumnType DbType { get; set; }
	public string TableName { get; set; }
	public string TableAlias { get; set; }
	public bool IsSystem { get; set; } = false;
	public SqlBuilderColumnType Type { get; set; } = SqlBuilderColumnType.Default;

	public string GetSelectString()
	{
		if (string.IsNullOrWhiteSpace(DataIdentity))
			return $"{TableAlias}.{DbName}";
		else
			return $"{TableAlias}.value AS {DbName}";
	}
}

internal enum SqlBuilderColumnType
{
	Default,
	Shared,
	Joined
}