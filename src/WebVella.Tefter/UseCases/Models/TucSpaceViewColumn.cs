using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.UseCases.Models;

public record TucSpaceViewColumn
{
	public Guid Id { get; set; }
	public Guid SpaceViewId { get; set; }
	public Guid? SelectedAddonId { get; set; } = null;
	public string QueryName { get; set; }
	public string Title { get; set; }
	public short Position { get; set; }
	public TucSpaceViewColumnType ColumnType { get; set; }
	public Type ComponentType { get; set; }
	public Dictionary<string, string> DataMapping { get; set; } = new();
	public string CustomOptionsJson { get; set; } = "{}";

	public TucSpaceViewColumn() { }

	public TucSpaceViewColumn(TfSpaceViewColumn model)
	{
		Id = model.Id;
		SpaceViewId = model.SpaceViewId;
		SelectedAddonId = model.SelectedAddonId;
		QueryName = model.QueryName;
		Title = model.Title;
		Position = model.Position;
		ColumnType = new TucSpaceViewColumnType(model.ColumnType);
		ComponentType = model.ComponentType;
		DataMapping = model.DataMapping;
		CustomOptionsJson = model.CustomOptionsJson;
	}

	//Column type should be get from GetAvailableSpaceViewColumnTypes()
	//and then matched by ID
	public TfSpaceViewColumn ToModel(ITfSpaceViewColumnType columnType)
	{
		return new TfSpaceViewColumn
		{
			Id = Id,
			SpaceViewId= SpaceViewId,
			SelectedAddonId= SelectedAddonId,
			QueryName = QueryName,
			Title = Title,
			Position = Position,
			ColumnType = columnType,
			ComponentType = ComponentType,
			DataMapping = DataMapping,
			CustomOptionsJson = CustomOptionsJson,
		};
	}

}
