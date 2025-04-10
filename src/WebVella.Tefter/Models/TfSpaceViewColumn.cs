namespace WebVella.Tefter.Models;

public class TfSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public string QueryName { get; set; }
	public string Title { get; set; }
	public string Icon { get; set; }
	public bool OnlyIcon { get; set; } = false;
	public short? Position { get; set; }
	public ITfSpaceViewColumnTypeAddon ColumnType { get; set; }
	public Type ComponentType { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";
	public string SettingsJson { get; set; } = "{}";
	public string FullTypeName { get; set; }
	public string FullComponentTypeName { get; set; }

}

[DboCacheModel]
[TfDboModel("tf_space_view_column")]
internal class TfSpaceViewColumnDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("space_view_id")]
	public Guid SpaceViewId { get; set; }

	[TfDboModelProperty("query_name")]
	public string QueryName { get; set; }

	[TfDboModelProperty("title")]
	public string Title { get; set; }

	[TfDboModelProperty("icon")]
	public string Icon { get; set; }

	[TfDboModelProperty("only_icon")]
	public bool OnlyIcon { get; set; } = false;

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("full_type_name")]
	public string FullTypeName { get; set; }

	[TfDboModelProperty("full_component_type_name")]
	public string FullComponentTypeName { get; set; }

	[TfDboModelProperty("data_mapping_json")]
	public string DataMappingJson { get; set; } = "{}";

	[TfDboModelProperty("custom_options_json")]
	public string CustomOptionsJson { get; set; } = "{}";

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";
}




