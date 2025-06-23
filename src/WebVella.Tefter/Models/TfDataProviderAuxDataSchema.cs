using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;


public class TfDataProviderAuxDataSchema
{
	public List<TfDataProviderAuxDataSchemaIdentity> DataIdentities { get; set; } = new();
}
