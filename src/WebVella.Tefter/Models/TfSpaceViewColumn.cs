namespace WebVella.Tefter.Models;

public record TfSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public string QueryName { get; set; } = null!;
	public string Title { get; set; } = null!;
	public string? Icon { get; set; } = null;
	public bool OnlyIcon { get; set; } = false;
	public short? Position { get; set; }
	public Guid TypeId { get; set; }
    public string? TypeOptionsJson { get; set; } = "{}";
    public TfSpaceViewColumnSettings Settings { get; set; } = new();
    public Dictionary<string, string?> DataMapping { get; set; } = new();
	public string? GetColumnNameFromDataMapping(){ 
		if(DataMapping.Keys.Count == 0) return null;
		return DataMapping[DataMapping.Keys.First()];
	}
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
	public string QueryName { get; set; } = null!;

	[TfDboModelProperty("title")]
	public string? Title { get; set; } = null;

	[TfDboModelProperty("icon")]
	public string? Icon { get; set; }

	[TfDboModelProperty("only_icon")]
	public bool OnlyIcon { get; set; } = false;

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("type_id")]
	public Guid TypeId { get; set; }
	
	[TfDboModelProperty("type_options_json")]
	public string TypeOptionsJson { get; set; } = "{}";

	[TfDboModelProperty("data_mapping_json")]
	public string DataMappingJson { get; set; } = "{}";

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";
}




