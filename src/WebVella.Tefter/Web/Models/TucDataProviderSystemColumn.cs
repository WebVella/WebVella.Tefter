namespace WebVella.Tefter.Web.Models;

public record TucDataProviderSystemColumn
{
	public string DbName { get; init; }
	public TucDatabaseColumnTypeInfo DbType { get; init; }
	
	public TucDataProviderSystemColumn() { }
	public TucDataProviderSystemColumn(TfDataProviderSystemColumn model)
	{
		DbName = model.DbName;
		DbType = new TucDatabaseColumnTypeInfo(model.DbType);
	}
	public TfDataProviderSystemColumn ToModel()
	{
		return new TfDataProviderSystemColumn
		{
			DbName = DbName,
			DbType = DbType.TypeValue,
		};
	}

}
