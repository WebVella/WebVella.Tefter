using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Web.Models;

public record TucSpaceNode
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

	public Guid? ComponentId { get; set; }
	[JsonIgnore]
	public Type ComponentType { get; set; }

	public string ComponentOptionsJson { get; set; } = "{}";
	public List<TucSpaceNode> ChildNodes { get; set; } = new();

	[JsonIgnore]
	public Action OnClick { get; set; }

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
		ComponentId = model.ComponentId;
		ComponentType = model.ComponentType;
		ComponentOptionsJson = model.ComponentOptionsJson;
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
			ComponentId = ComponentId,
			ComponentType = ComponentType,
			ComponentOptionsJson = ComponentOptionsJson,
			ChildNodes = ChildNodes.Select(x=> x.ToModel()).ToList(),
			//ComponentType is a getter
		};
	}

	public TucMenuItem ToMenuItem(Action<TucMenuItem> postProcess = null){ 
		var item = new TucMenuItem
		{ 
			Id = TfConverters.ConvertGuidToHtmlElementId(Id),
			Expanded = false,
			IconCollapsed = TfConstants.GetIcon(Icon),
			IconExpanded = TfConstants.GetIcon(Icon),
			Text = Name,
			Nodes = ChildNodes.Select(x=> x.ToMenuItem(postProcess)).ToList(),
			OnClick = null,
			OnExpand = null,
			Data = this,
			Url = Type == TfSpaceNodeType.Folder ? null : String.Format(TfConstants.SpaceNodePageUrl,SpaceId,Id),
			Description = null
		};

		if(postProcess is not null){ 
			postProcess(item);
		}

		return item;
	}

	

}
