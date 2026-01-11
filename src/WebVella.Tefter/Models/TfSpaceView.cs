namespace WebVella.Tefter.Models;

public class TfSpaceView
{
	public Guid Id { get; set; }
	public Guid DatasetId { get; set; }
	public string SpaceDataName { get; set; } = null!;
	public string Name { get; set; } = null!;
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
	public List<TfSpaceViewPreset> PresetsWithParents => Presets.FillSpaceViewPresetParents();

	public bool CanCreateRecords(TfUser user)
	{
		if (Settings is not null && user.Roles is not null
			&& user.Roles.Any(x => Settings.CanCreateRoles.Contains(x.Id)))
			return true;
		return false;
	}

	public bool CanUpdateRecords(TfUser user)
	{
		if (Settings is not null && user.Roles is not null
			&& user.Roles.Any(x => Settings.CanUpdateRoles.Contains(x.Id)))
			return true;
		return false;
	}

	public bool CanDeleteRecords(TfUser user)
	{
		if (Settings is not null && user.Roles is not null
			&& user.Roles.Any(x => Settings.CanDeleteRoles.Contains(x.Id)))
			return true;
		return false;
	}

}

[DboCacheModel]
[TfDboModel("tf_space_view")]
public class TfSpaceViewDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("space_data_id")]
	public Guid SpaceDataId { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";

	[TfDboModelProperty("presets_json")]
	public string PresetsJson { get; set; } = "[]";
}


public class TfSpaceViewCreateModel
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public Guid? DatasetId { get; set; } = null;
	public List<TfSpaceViewPreset> Presets { get; set; } = new();
	public TfSpaceViewSettings Settings { get; set; } = new TfSpaceViewSettings();

	public Guid? TemplateId { get; set; } = null;
}

public record TfSpaceViewPreset
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; }

	[JsonPropertyName("parent_id")]
	public Guid? ParentId { get; set; }

	[JsonPropertyName("name")]
	public string? Name { get; set; }

	[JsonPropertyName("search")]
	public string? Search { get; set; }

	[JsonPropertyName("filters")]
	public List<TfFilterBase> Filters { get; set; } = new();

	[JsonPropertyName("sort_orders")]
	public List<TfSort> SortOrders { get; set; } = new();

	[JsonPropertyName("pages")]
	public List<TfSpaceViewPreset> Presets { get; set; } = new();

	[JsonPropertyName("is_group")]
	public bool IsGroup { get; set; } = false;

	[JsonPropertyName("color")]
	public TfColor? Color { get; set; } = TfConstants.DefaultThemeColor;

	[JsonPropertyName("icon")] public string Icon { get; set; } = null!;
}