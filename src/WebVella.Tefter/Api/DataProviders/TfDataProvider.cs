using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter;

public class TfDataProvider
{
	public Guid Id { get; set; }

	[Required]
	public string Name { get; set; }
	public int Index { get; set; }
	public string SettingsJson { get; set; }
	public ReadOnlyCollection<TfDataProviderColumn> Columns { get; set; }
	public ITfDataProviderType ProviderType { get; set; }
	public ReadOnlyCollection<string> SupportedSourceDataTypes => ProviderType.GetSupportedSourceDataTypes();

}
