namespace WebVella.Tefter.Models;
internal record SqlBuilderJoinData
{
	public TfDataProvider Provider { get; set; }
	public string DataIdentity { get; set; }
	public string TableAlias { get; set; }
	public string TableName { get; set; }
	public TfDataProvider DataProvider { get; set; }
	public List<SqlBuilderColumn> Columns { get; set; } = new();
	public string GetSelectString(string mainProviderAlias)
	{
		string columns = string.Join($",", Columns.Select(x => x.DbName).ToList());

		var columnDataIdentity = DataIdentity;
		if (columnDataIdentity != TfConstants.TF_ROW_ID_DATA_IDENTITY)
			columnDataIdentity = "tf_ide_" + columnDataIdentity;


		return $"(SELECT  COALESCE( array_to_json( array_agg( row_to_json(d) )), '[]') FROM ( " +
			$"SELECT {columns} FROM {TableName} WHERE {TableName}.tf_ide_{DataIdentity} = {mainProviderAlias}.{columnDataIdentity} " +
			$") d )::jsonb AS  \"jp${TableName}${DataIdentity}\"";
	}
}