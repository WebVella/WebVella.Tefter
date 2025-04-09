namespace WebVella.Tefter.Models;

public class TfSpaceNode
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; } = null;
	public Guid SpaceId { get; set; }
	public TfSpaceNodeType Type { get; set; } = TfSpaceNodeType.Page;
	public string Name { get; set; }
	public string Icon { get; set; } = TfConstants.PageIconString;
	public short? Position { get; set; }
	public Guid? ComponentId { get; init; }
	public Type ComponentType { get; internal set; }
	public string ComponentOptionsJson { get; set; } = "{}";
	public List<TfSpaceNode> ChildNodes { get; set; } = new();
	public TfSpaceNode ParentNode { get; set; } = null;

	internal List<TfSpaceNode> GetChildNodesPlainList()
	{
		List<TfSpaceNode> result = new List<TfSpaceNode>();
		Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

		foreach (var node in ChildNodes)
			queue.Enqueue(node);

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();

			result.Add(node);

			foreach (var childNode in node.ChildNodes)
				queue.Enqueue(childNode);

		}

		return result;
	}
	public override string ToString()
	{
		return $"{Name} (pos:{Position}; par:{ParentNode?.Name})";
	}
}


[DboCacheModel]
[TfDboModel("tf_space_page")]
public class TfSpaceNodeDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("parent_id")]
	public Guid? ParentId { get; set; } = null;

	[TfDboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[TfDboModelProperty("type")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfSpaceNodeType>))]
	public TfSpaceNodeType Type { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboModelProperty("icon")]
	public string Icon { get; set; } = null;

	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("component_id")]
	public Guid? ComponentId { get; set; }

	[TfDboModelProperty("component_settings_json")]
	public string ComponentSettingsJson { get; set; } = "{}";
}


