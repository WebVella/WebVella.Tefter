namespace WebVella.Tefter;

public class TfSpaceView
{
	public Guid Id { get; set; }
	public Guid SpaceDataId { get; set; }
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.Report;
	public string Name { get; set; }
	public short Position { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public List<string> Groups { get; set; } = new();
}

[DboCacheModel]
[DboModel("space_view")]
public class TfSpaceViewDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("space_data_id")]
	public Guid SpaceDataId { get; set; }

	[DboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[DboModelProperty("type")]
	[DboTypeConverter(typeof(EnumPropertyConverter<TfSpaceViewType>))]
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.Report;

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("position")]
	public short Position { get; set; }

	[DboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";

	[DboModelProperty("presets_json")]
	public string PresetsJson { get; set; } = "[]";

	[DboModelProperty("groups_json")]
	public string GroupsJson { get; set; } = "[]";
}


public class TfCreateSpaceViewExtended
{
	public Guid Id { get; set; }
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.Report;
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
}

public class TfSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; }
	
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
}

public enum TfSpaceViewType
{
	Report = 0,
	Form = 1,
	Chart = 2,
	Dashboard = 3
}
