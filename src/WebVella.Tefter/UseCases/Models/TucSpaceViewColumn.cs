using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public Guid? SelectedAddonId { get; set; } = null;

	[Required(ErrorMessage = "required")]
	public string QueryName { get; set; }
	[Required(ErrorMessage = "required")]
	public string Title { get; set; }
	public short? Position { get; set; }

	[Required(ErrorMessage = "required")]
	public TucSpaceViewColumnType ColumnType { get; set; }

	[Required(ErrorMessage = "required")]
	public Type ComponentType { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";
	public TucSpaceViewColumnSettings Settings { get; set; } = new TucSpaceViewColumnSettings();

	public string HeaderStyle
	{
		get
		{
			var sb = new StringBuilder();
			//if (Settings is not null)
			//{
			//	if(Settings.BackgroundColor != OfficeColor.Default){ 
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
				if(Settings.Color != OfficeColor.Default){ 
					sb.Append($"color:{Settings.Color.ToAttributeValue()};");
				}
				if(Settings.BackgroundColor != OfficeColor.Default){ 
					sb.Append($"background: {Settings.BackgroundColor.ToAttributeValue()}15");
					//sb.Append($"background:linear-gradient(to right, {Settings.BackgroundColor.ToAttributeValue()}50, {Settings.BackgroundColor.ToAttributeValue()}50)," +
					//$"linear-gradient(to right, var(--neutral-fill-layer-rest),var(--neutral-fill-layer-rest));");
				}
			}
			if(sb.Length == 0) sb.Append("display:none;");

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
		SelectedAddonId = model.SelectedAddonId;
		QueryName = model.QueryName;
		Title = model.Title;
		Position = model.Position;
		ColumnType = model.ColumnType is not null ? new TucSpaceViewColumnType(model.ColumnType) : null;
		ComponentType = model.ComponentType;
		DataMapping = model.DataMapping;
		CustomOptionsJson = model.CustomOptionsJson;
		FullTypeName = model.FullTypeName;
		FullComponentTypeName = model.FullComponentTypeName;
		Settings = new TucSpaceViewColumnSettings();
		if (!String.IsNullOrWhiteSpace(model.SettingsJson) && model.SettingsJson.StartsWith("{")
		 && model.SettingsJson.EndsWith("}"))
		{
			Settings = JsonSerializer.Deserialize<TucSpaceViewColumnSettings>(model.SettingsJson);
		}
	}

	//Column type should be get from GetAvailableSpaceViewColumnTypes()
	//and then matched by ID
	public TfSpaceViewColumn ToModel(ITfSpaceViewColumnType columnType)
	{
		return new TfSpaceViewColumn
		{
			Id = Id,
			SpaceViewId = SpaceViewId,
			SelectedAddonId = SelectedAddonId,
			QueryName = QueryName,
			Title = Title,
			Position = Position,
			ColumnType = columnType,
			ComponentType = ComponentType,
			DataMapping = DataMapping,
			CustomOptionsJson = CustomOptionsJson,
			FullTypeName = FullTypeName,
			FullComponentTypeName = FullComponentTypeName,
			SettingsJson = JsonSerializer.Serialize(Settings)
		};
	}

}
