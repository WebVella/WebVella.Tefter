using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.DataProviders.Csv;

internal class CsvDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }	
}
