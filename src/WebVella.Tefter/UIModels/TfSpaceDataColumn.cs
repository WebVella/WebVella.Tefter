using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public record TfSpaceDataColumn
{
	public string? DataIdentity { get; set; } = null;
	public string? ColumnName { get; set; }
	public string? SourceColumnName { get; set; }
	public string? SourceName { get; set; }
	public string? SourceCode { get; set; }
	public TfAuxDataSourceType SourceType { get; set; } = TfAuxDataSourceType.PrimatyDataProvider;
	public TfDatabaseColumnType DbType { get; set; }
	
}
