namespace WebVella.Tefter.Models;

public class TfSpaceView
{
	public Guid Id { get; set; }
	public Guid SpaceDataId { get; set; }
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;
	public string Name { get; set; }
	public short Position { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public List<string> Groups { get; set; } = new();
}

[DboCacheModel]
[TfDboModel("space_view")]
public class TfSpaceViewDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("space_data_id")]
	public Guid SpaceDataId { get; set; }

	[TfDboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[TfDboModelProperty("type")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfSpaceViewType>))]
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";

	[TfDboModelProperty("presets_json")]
	public string PresetsJson { get; set; } = "[]";

	[TfDboModelProperty("groups_json")]
	public string GroupsJson { get; set; } = "[]";
}


public class TfCreateSpaceViewExtended
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;
	public short Position { get; set; }
	public string Name { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public List<string> Groups { get; set; } = new();
	public Guid? SpaceDataId { get; set; } = null;
	public string NewSpaceDataName { get; set; } = null;
	public Guid? DataProviderId { get; set; } = null;
	public bool AddSystemColumns { get; set; } = false;
	public bool AddProviderColumns { get; set; } = true;
	public bool AddSharedColumns { get; set; } = true;
	public bool AddDataSetColumns { get; set; } = true;
}

public class TfSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; }

	[JsonPropertyName("parent_id")]
	public Guid? ParentId { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("filters")]
	public List<TfFilterBase> Filters { get; set; } = new();

	[JsonPropertyName("sort_orders")]
	public List<TfSort> SortOrders { get; set; } = new();

	[JsonPropertyName("nodes")]
	public List<TfSpaceViewPreset> Nodes { get; set; } = new();

	[JsonPropertyName("is_group")]
	public bool IsGroup { get; set; } = false;

	[JsonPropertyName("color")]
	public OfficeColor Color { get; set; } = OfficeColor.Default;

	[JsonPropertyName("icon")]
	public string Icon { get; set; }
}

public enum TfSpaceViewType
{
	DataGrid = 0,
	Form = 1,
	Chart = 2,
	Dashboard = 3
}
