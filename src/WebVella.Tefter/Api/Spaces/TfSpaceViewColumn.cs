namespace WebVella.Tefter;

public class TfSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public Guid? SelectedAddonId { get; set; } = null;
	public string QueryName { get; set; }
	public string Title { get; set; }
	public short? Position { get; set; }
	public ITfSpaceViewColumnType ColumnType { get; set; }
	public Type ComponentType { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";
	public string FullTypeName { get; set; }
	public string FullComponentTypeName { get; set; }

}

[DboCacheModel]
[DboModel("space_view_column")]
internal class TfSpaceViewColumnDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("space_view_id")]
	public Guid SpaceViewId { get; set; }
	
	[DboModelProperty("selected_addon_id")]
	public Guid? SelectedAddonId { get; set; } = null;

	[DboModelProperty("query_name")]
	public string QueryName { get; set; }

	[DboModelProperty("title")]
	public string Title { get; set; }

	[DboModelProperty("position")]
	public short Position { get; set; }

	[DboModelProperty("full_type_name")]
	public string FullTypeName { get; set; }

	[DboModelProperty("full_component_type_name")]
	public string FullComponentTypeName { get; set; }

	[DboModelProperty("data_mapping_json")]
	public string DataMappingJson { get; set; } = "{}";

	[DboModelProperty("custom_options_json")]
	public string CustomOptionsJson { get; set; } = "{}";
}



