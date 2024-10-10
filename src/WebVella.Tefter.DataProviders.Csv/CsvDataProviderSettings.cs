using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.DataProviders.Csv;

internal class CsvDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }

	[Required]
	public CsvDataProviderSettingsDelimiter Delimter { get; set; } = CsvDataProviderSettingsDelimiter.Comma;

	[Required]
	public string CultureName { get; set; } = Thread.CurrentThread.CurrentCulture.Name;

	[Required]
	public CsvDataProviderSettingsAdvanced AdvancedSetting { get; set; } = new CsvDataProviderSettingsAdvanced();
}

internal class CsvDataProviderSettingsAdvanced
{
	public Dictionary<string,string> ColumnImportParseFormat { get; set;} = new();
}

internal enum CsvDataProviderSettingsDelimiter
{
	Comma = 0,
	Semicolon = 1,
	Tab = 2
}