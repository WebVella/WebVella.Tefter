using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter;

public record TfDataProviderModel
{
	[Required]
	public Guid Id { get; set; }
	[Required]
	public string Name { get; set; }
	[Required]
	public ITfDataProviderType ProviderType { get; set; }

	public string CompositeKeyPrefix { get; set; } = null;

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