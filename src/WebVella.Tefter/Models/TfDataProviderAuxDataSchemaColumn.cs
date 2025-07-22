using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public class TfDataProviderAuxDataSchemaColumn
{
	public TfDataIdentity? DataIdentity { get; set; } = default!;
	public string DbName { get; set; } = default!;

	//Filled when it is a shared column
	public TfSharedColumn? SharedColumn { get; set; } = null;
	
	//Filled when it is a provider
	public TfDataProvider? DataProvider { get;set;} = null;
	public TfDataProviderColumn? DataProviderColumn { get;set;} = null;
}
