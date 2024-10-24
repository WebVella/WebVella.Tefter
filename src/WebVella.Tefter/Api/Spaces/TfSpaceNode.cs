namespace WebVella.Tefter;

public class TfSpaceNode
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; } = null;
	public Guid SpaceId { get; set; }
	public TfSpaceNodeType Type { get; set; } = TfSpaceNodeType.Page;
	public string Name { get; set; }
	public string Icon { get; set; } = TfConstants.PageIconString;
	public short? Position { get; set; }
	public string ComponentType { get; set; }
	public string ComponentSettingsJson { get; set; } = "{}";
	public List<TfSpaceNode> ChildNodes { get; set; } = new();
	public TfSpaceNode ParentNode { get; set; } = null;

	internal List<TfSpaceNode> GetChildNodesPlainList()
	{
		List<TfSpaceNode> result = new List<TfSpaceNode>();
		Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

		foreach(var node in ChildNodes)
			queue.Append(node);

		while(queue.Count > 0)
		{
			var node = queue.Dequeue();
		
			result.Add(node);

			foreach (var childNode in ChildNodes)
				queue.Append(childNode);

		}

		return result;
	}
	public override string ToString()
	{
		return $"{Name} (pos:{Position}; par:{ParentNode?.Name})";
	}
}


[DboCacheModel]
[DboModel("space_node")]
public class TfSpaceNodeDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("parent_id")]
	public Guid? ParentId { get; set; } = null;

	[DboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[DboModelProperty("type")]
	[DboTypeConverter(typeof(EnumPropertyConverter<TfSpaceNodeType>))]
	public TfSpaceNodeType Type { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboModelProperty("icon")]
	public string Icon { get; set; } = null;

	[DboModelProperty("position")]
	public short Position { get; set; }

	[DboModelProperty("component_type")]
	public string ComponentType { get; set; }

	[DboModelProperty("component_settings_json")]
	public string ComponentSettingsJson { get; set; } = "{}";
}


