namespace WebVella.Tefter.Models;

public record TfSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public string QueryName { get; set; }
	public string Title { get; set; }
	public string Icon { get; set; }
	public bool OnlyIcon { get; set; } = false;
	public short? Position { get; set; }
	public Guid TypeId { get; set; }
	public Guid ComponentId { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string ComponentOptionsJson { get; set; } = "{}";
	public TfSpaceViewColumnSettings Settings { get; set; } = new();
	public string HeaderStyle
	{
		get
		{
			var sb = new StringBuilder();
			//if (Settings is not null)
			//{
			//	if(Settings.BackgroundColor != TfColor.Black){ 
			//		sb.Append($"background:linear-gradient(to right, {Settings.BackgroundColor.ToAttributeValue()}35, {Settings.BackgroundColor.ToAttributeValue()}35)," +
			//		$"linear-gradient(to right, var(--neutral-fill-layer-rest),var(--neutral-fill-layer-rest));");
			//	}
			//}

			return sb.ToString();
		}
	}

	public string CellColorStyle
	{
		get
		{
			var sb = new StringBuilder();
			if (Settings is not null)
			{
				if (Settings.Color != TfColor.Black)
				{
					sb.Append($"color: var(--tf-td-color-{Settings.Color.GetAttribute().Name});");
				}
			}
			return sb.ToString();
		}
	}

	public string CellFillColorStyle
	{
		get
		{
			var sb = new StringBuilder();
			if (Settings is not null)
			{
				if (Settings.BackgroundColor != TfColor.Black)
				{
					sb.Append($"background: var(--tf-td-fill-{Settings.BackgroundColor.GetAttribute().Name});");
				}
			}
			if (sb.Length == 0) sb.Append("display:none;");

			return sb.ToString();
		}
	}

	//Render only helpers
	public TfSortDirection? PersonalizedSort { get; set; } = null;

	public string? GetColumnNameFromDataMapping(){ 
		if(DataMapping is null || DataMapping.Keys.Count == 0) return null;
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
	public string QueryName { get; set; }

	[TfDboModelProperty("title")]
	public string Title { get; set; }

	[TfDboModelProperty("icon")]
	public string Icon { get; set; }

	[TfDboModelProperty("only_icon")]
	public bool OnlyIcon { get; set; } = false;

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("type_id")]
	public Guid TypeId { get; set; }

	[TfDboModelProperty("component_id")]
	public Guid ComponentId { get; set; }

	[TfDboModelProperty("data_mapping_json")]
	public string DataMappingJson { get; set; } = "{}";

	[TfDboModelProperty("custom_options_json")]
	public string ComponentOptionsJson { get; set; } = "{}";

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = "{}";
}




