using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceNode : ITfSpaceNodeComponentContext
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; } = null;
	public Guid SpaceId { get; set; }
	public TfSpaceNodeType Type { get; set; } = TfSpaceNodeType.Page;
	[Required]
	public string Name { get; set; }
	[Required]
	public string Icon { get; set; } = TfConstants.PageIconString;
	public short? Position { get; set; }
	public string ComponentType { get; set; }
	public string ComponentSettingsJson { get; set; } = "{}";
	public List<TucSpaceNode> ChildNodes { get; set; } = new();
	public TucSpaceNode() { }
	public TucSpaceNode(TfSpaceNode model)
	{
		Id = model.Id;
		ParentId = model.ParentId;
		SpaceId = model.SpaceId;
		Type = model.Type;
		Name = model.Name; 
		Icon = model.Icon;
		Position = model.Position;
		ComponentType = model.ComponentType;
		ComponentSettingsJson = model.ComponentSettingsJson;
		ChildNodes = model.ChildNodes.Select(x => new TucSpaceNode(x)).ToList();
	}
	public TfSpaceNode ToModel()
	{
		return new TfSpaceNode
		{
			Id = Id,
			ParentId = ParentId,
			SpaceId = SpaceId,
			Type = Type,
			Name = Name,
			Icon = Icon,
			Position = Position,
			ComponentType = ComponentType,
			ComponentSettingsJson = ComponentSettingsJson,
			ChildNodes = ChildNodes.Select(x=> x.ToModel()).ToList(),
		};
	}

}
