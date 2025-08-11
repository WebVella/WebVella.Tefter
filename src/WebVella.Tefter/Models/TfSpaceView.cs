namespace WebVella.Tefter.Models;

public class TfSpaceView
{
	public Guid Id { get; set; }
	public Guid SpaceDataId { get; set; }
	public string SpaceDataName { get; set; } = default!;
	public Guid SpaceId { get; set; }
	public TfSpaceViewType Type { get; set; } = TfSpaceViewType.DataGrid;
	public string Name { get; set; } = default!;
	public short Position { get; set; }
	public string SettingsJson { get; set; } = "{}";
	public TfSpaceViewSettings Settings
	{
		get
		{
			if (!String.IsNullOrWhiteSpace(SettingsJson) && SettingsJson.StartsWith("{")
				 && SettingsJson.EndsWith("}"))
			{
				return JsonSerializer.Deserialize<TfSpaceViewSettings>(SettingsJson) ?? new TfSpaceViewSettings();
			}
			return new TfSpaceViewSettings();
		}
	}
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
}

[DboCacheModel]
[TfDboModel("tf_space_view")]
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
	public TfSpaceViewDataSetType DataSetType { get; set; } = TfSpaceViewDataSetType.New;
	public Guid? SpaceDataId { get; set; } = null;
	public string? NewSpaceDataName { get; set; } = null;
	public Guid? DataProviderId { get; set; } = null;
	public bool AddSystemColumns { get; set; } = false;
	public bool AddProviderColumns { get; set; } = true;
	public bool AddSharedColumns { get; set; } = true;
	public bool AddDataSetColumns { get; set; } = true;
	public TfSpaceViewSettings Settings { get; set; } = new TfSpaceViewSettings();
}

public record TfSpaceViewPreset
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

	[JsonPropertyName("pages")]
	public List<TfSpaceViewPreset> Presets { get; set; } = new();

	[JsonPropertyName("is_group")]
	public bool IsGroup { get; set; } = false;

	[JsonPropertyName("color")]
	public TfColor Color { get; set; } = TfColor.Emerald500;

	[JsonPropertyName("icon")]
	public string Icon { get; set; }
}

public enum TfSpaceViewType
{
	[Description("Datagrid")]
	DataGrid = 0,
	[Description("Form")]
	Form = 1,
	[Description("Chart")]
	Chart = 2,
	[Description("Dashboard")]
	Dashboard = 3
}

public enum TfSpaceViewDataSetType
{
	[Description("new dataset")]
	New = 0,
	[Description("existing dataset")]
	Existing = 1
}

public enum TfSpaceViewSetType
{
	[Description("new view")]
	New = 0,
	[Description("existing view")]
	Existing = 1
}