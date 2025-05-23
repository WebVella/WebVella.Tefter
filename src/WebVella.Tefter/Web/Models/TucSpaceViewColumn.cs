﻿using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }

	[Required(ErrorMessage = "required")]
	public string QueryName { get; set; }
	[Required(ErrorMessage = "required")]
	public string Title { get; set; }
	public string Icon { get; set; }
	public bool OnlyIcon { get; set; } = false;
	public short? Position { get; set; }
	[Required(ErrorMessage = "required")]
	public Guid TypeId { get; set; }
	[Required(ErrorMessage = "required")]
	public Guid ComponentId { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string ComponentOptionsJson { get; set; } = "{}";
	public TucSpaceViewColumnSettings Settings { get; set; } = new TucSpaceViewColumnSettings();

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

	public string FullTypeName { get; set; }
	public string FullComponentTypeName { get; set; }

	public TucSpaceViewColumn() { }

	public TucSpaceViewColumn(TfSpaceViewColumn model)
	{
		Id = model.Id;
		SpaceViewId = model.SpaceViewId;
		QueryName = model.QueryName;
		Title = model.Title;
		Icon = model.Icon;
		OnlyIcon = model.OnlyIcon;
		Position = model.Position;
		TypeId = model.TypeId;
		ComponentId = model.ComponentId;
		DataMapping = model.DataMapping;
		ComponentOptionsJson = model.ComponentOptionsJson;
		Settings = new TucSpaceViewColumnSettings();
		if (!String.IsNullOrWhiteSpace(model.SettingsJson) && model.SettingsJson.StartsWith("{")
		 && model.SettingsJson.EndsWith("}"))
		{
			Settings = JsonSerializer.Deserialize<TucSpaceViewColumnSettings>(model.SettingsJson);
		}
	}

	//Column type should be get from GetAvailableSpaceViewColumnTypes()
	//and then matched by ID
	public TfSpaceViewColumn ToModel(ITfSpaceViewColumnTypeAddon columnType)
	{
		return new TfSpaceViewColumn
		{
			Id = Id,
			SpaceViewId = SpaceViewId,
			QueryName = QueryName,
			Title = Title,
			Icon = Icon,
			OnlyIcon = OnlyIcon,
			Position = Position,
			DataMapping = DataMapping,
			ComponentOptionsJson = ComponentOptionsJson,
			TypeId = TypeId,
			ComponentId = ComponentId,
			SettingsJson = JsonSerializer.Serialize(Settings)
		};
	}
	public TfSpaceViewColumn ToModel()
	{
		return new TfSpaceViewColumn
		{
			Id = Id,
			SpaceViewId = SpaceViewId,
			QueryName = QueryName,
			Title = Title,
			Icon = Icon,
			OnlyIcon = OnlyIcon,
			Position = Position,
			TypeId = TypeId,
			ComponentId = ComponentId,
			DataMapping = DataMapping,
			ComponentOptionsJson = ComponentOptionsJson,
			SettingsJson = JsonSerializer.Serialize(Settings)
		};
	}

}
