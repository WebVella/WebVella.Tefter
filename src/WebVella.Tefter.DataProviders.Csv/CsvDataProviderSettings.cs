using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProviderSettings
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

public class CsvDataProviderSettingsAdvanced
{
	public Dictionary<string,string> ColumnImportParseFormat { get; set;} = new();
}

public enum CsvDataProviderSettingsDelimiter
{
	Comma = 0,
	Semicolon = 1,
	Tab = 2
}