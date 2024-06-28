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
	public ReadOnlyCollection<TfDataProviderSharedKey> SharedKeys { get; set; }
	public ITfDataProviderType ProviderType { get; set; }
	public ReadOnlyCollection<string> SupportedSourceDataTypes => ProviderType.GetSupportedSourceDataTypes();

}

public record TfDataProviderModel
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public ITfDataProviderType ProviderType { get; set; }

	public string SettingsJson { get; set; } = null;
}


[DboCacheModel]
[DboModel("data_provider")]
internal record TfDataProviderDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboAutoIncrementModel]
	[DboModelProperty("index")]
	public int Index { get; set; }

	[DboModelProperty("type_id")]
	public Guid TypeId { get; set; }

	[DboModelProperty("type_name")]
	public string TypeName { get; set; }

	[DboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = null;
}