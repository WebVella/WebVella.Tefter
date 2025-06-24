namespace WebVella.Tefter.Models;

internal record SqlBuilderSharedColumnData
{
	public string DbName { get; set; }
	public TfDatabaseColumnType DbType { get; set; }
	public string DataIdentity { get; set; }
	public string TableAlias { get; set; }
	public string TableName { get; set; }
	public SqlBuilderColumn BuilderColumnInfo { get; set; }

	public string GetSelectString()
	{
		return $"{TableAlias}.value AS {DbName}";
	}
}