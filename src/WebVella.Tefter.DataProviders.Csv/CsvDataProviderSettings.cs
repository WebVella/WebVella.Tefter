using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.DataProviders.Csv;

internal class CsvDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }

	[Required]
	public CsvDataProviderSettingsDelimter Delimter { get; set; } = CsvDataProviderSettingsDelimter.Comma;
}

internal enum CsvDataProviderSettingsDelimter
{
	Comma = 0,
	Tab = 1
}