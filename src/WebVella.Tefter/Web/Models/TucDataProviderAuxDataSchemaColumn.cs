namespace WebVella.Tefter.Web.Models;

public record TucDataProviderAuxDataSchemaColumn
{
	//filled in only for AllColumns case of the schema get property
	public TucDataIdentity DataIdentity { get; set; }
	public string DbName { get; set; }

	//Filled when it is a shared column
	public TucSharedColumn SharedColumn { get; set; } = null;

	//Filled when it is a provider
	public TucDataProvider DataProvider { get; set; } = null;
	public TucDataProviderColumn DataProviderColumn { get; set; } = null;
	public TucDataProviderAuxDataSchemaColumn() { }
	public TucDataProviderAuxDataSchemaColumn(TfDataProviderAuxDataSchemaColumn model)
	{
		DataIdentity = null;
		DbName = model.DbName;
		SharedColumn = model.SharedColumn is null ? null : new TucSharedColumn(model.SharedColumn);
		DataProvider = model.DataProvider is null ? null : new TucDataProvider(model.DataProvider);
		DataProviderColumn = model.DataProviderColumn is null ? null : new TucDataProviderColumn(model.DataProviderColumn);
	}

}
