using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace WebVella.Tefter.DataProviders.Excel;

public class ExcelDataProviderSettings
{
	[Required]
	public string Filepath { get; set; }

    [Required]
    public string CultureName { get; set; } = Thread.CurrentThread.CurrentCulture.Name;

    [Required]
    public ExcelDataProviderSettingsAdvanced AdvancedSetting { get; set; } = new ExcelDataProviderSettingsAdvanced();
}

public class ExcelDataProviderSettingsAdvanced
{
    public Dictionary<string, string> ColumnImportParseFormat { get; set; } = new();
}
