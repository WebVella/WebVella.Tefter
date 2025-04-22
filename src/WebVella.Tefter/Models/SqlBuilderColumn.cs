namespace WebVella.Tefter.Models;
internal record SqlBuilderColumn
{
	public Guid Id { get; set; }
	public string DbName { get; set; }
	public string JoinKeyDbName { get; set; }
	public TfDatabaseColumnType DbType { get; set; }
	public string TableName { get; set; }
	public string TableAlias { get; set; }
	public bool IsSystem { get; set; } = false;

	public string GetSelectString()
	{
		if (string.IsNullOrWhiteSpace(JoinKeyDbName))
			return $"{TableAlias}.{DbName}";
		else
			return $"{TableAlias}.value AS {DbName}";
	}
}