using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.Seeds.SampleDataProvider;

internal class SampleDataProviderSettings
{
	[Required]
	public string SampleSetting { get; set; }

	[Required]
	public SampleDataProviderSettingsAdvanced AdvancedSetting { get; set; } = new();
}

internal class SampleDataProviderSettingsAdvanced
{
	public Dictionary<string,string> SampleSettings { get; set;} = new();
}
