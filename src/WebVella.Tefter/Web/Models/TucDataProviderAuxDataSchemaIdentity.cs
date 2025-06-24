namespace WebVella.Tefter.Web.Models;

public record TucDataProviderAuxDataSchemaIdentity
{
	public TucDataIdentity DataIdentity { get; set; }
	public List<TucDataProviderAuxDataSchemaColumn> Columns { get; set; } = new();
	public TucDataProviderAuxDataSchemaIdentity() { }
	public TucDataProviderAuxDataSchemaIdentity(TfDataProviderAuxDataSchemaIdentity model)
	{
		DataIdentity = new TucDataIdentity(model.DataIdentity);
		Columns = model.Columns.Select(x=> new TucDataProviderAuxDataSchemaColumn(x)).ToList();
	}


}
