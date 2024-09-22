using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebVella.Tefter.DataProviders.Csv;

internal class CsvDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }

	[Required]
	public CsvDataProviderSettingsDelimter Delimter { get; set; } = CsvDataProviderSettingsDelimter.Comma;

	[Required]
	public string CultureName { get; init; } = string.Empty;
}

internal enum CsvDataProviderSettingsDelimter
{
	Comma = 0,
	Tab = 1
}