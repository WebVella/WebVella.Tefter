namespace WebVella.Tefter;

[DboCacheModel]
[DboModel("data_provider")]
internal record DataProviderDbo
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

	[DboModelProperty("composite_key_prefix")]
	public string CompositeKeyPrefix { get; set; } = null;

	[DboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = null;
}