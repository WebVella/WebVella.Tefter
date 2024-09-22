using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebVella.Tefter.DataProviders.Csv;

internal class CsvDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }

	[Required]
	public CsvDataProviderSettingsDelimiter Delimter { get; set; } = CsvDataProviderSettingsDelimiter.Comma;

	[Required]
	public string CultureName { get; set; } = Thread.CurrentThread.CurrentCulture.Name;
}

internal enum CsvDataProviderSettingsDelimiter
{
	Comma = 0,
	Semicolon = 1,
	Tab = 2
}