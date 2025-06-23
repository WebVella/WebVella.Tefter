using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public class TfDataProviderAuxDataSchemaColumn
{
	public string DbName { get; set; }

	//Filled when it is a shared column
	public TfSharedColumn SharedColumn { get; set; } = null;
	
	//Filled when it is a provider
	public TfDataProvider DataProvider { get;set;} = null;
	public TfDataProviderColumn DataProviderColumn { get;set;} = null;
}
