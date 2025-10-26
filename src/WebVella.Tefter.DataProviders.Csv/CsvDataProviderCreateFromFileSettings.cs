using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.DataProviders.Csv;

public class CsvDataProviderCreateFromFileSettings
{
	[Required]
	public CsvDataProviderSettingsDelimiter Delimter { get; set; } = CsvDataProviderSettingsDelimiter.Comma;

	[Required]
	public string CultureName { get; set; } = Thread.CurrentThread.CurrentCulture.Name;

}