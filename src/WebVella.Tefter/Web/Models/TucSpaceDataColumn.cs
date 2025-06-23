using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceDataColumn
{
	public Guid? Id { get; set; }
	public TucDataIdentity DataIdentity { get; set; } = null;
	public string ColumnName { get; set; }
	public string SourceColumnName { get; set; }
	public string SourceName { get; set; }
	public string SourceCode { get; set; }
	public TfAuxDataSourceType SourceType { get; set; } = TfAuxDataSourceType.PrimatyDataProvider;
	public TucDatabaseColumnTypeInfo DbType { get; set; }
	
}
