using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;


public class TfDataProviderAuxDataSchemaIdentity
{
	public TfDataIdentity DataIdentity { get; set; } = null!;
	public List<TfDataProviderAuxDataSchemaColumn> Columns { get; set; } = new();
}
