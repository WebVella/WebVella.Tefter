namespace WebVella.Tefter.Web.Models;

public record TucDataProviderAuxDataSchema
{
	public List<TucDataProviderAuxDataSchemaIdentity> Identities { get; set; } = new();

	[JsonIgnore]
	public List<TucDataProviderAuxDataSchemaColumn> AllColumns
	{
		get
		{
			var result = new List<TucDataProviderAuxDataSchemaColumn>();
			foreach (var identity in Identities)
			{
				foreach (var column in identity.Columns)
				{
					column.DataIdentity = identity.DataIdentity;
					result.Add(column);
				}
			}
			return result;
		}
	}

	public TucDataProviderAuxDataSchema() { }
	public TucDataProviderAuxDataSchema(TfDataProviderAuxDataSchema model)
	{
		Identities = model.DataIdentities.Select(x => new TucDataProviderAuxDataSchemaIdentity(x)).ToList();
	}


}
