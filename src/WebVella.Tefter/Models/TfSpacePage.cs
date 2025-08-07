namespace WebVella.Tefter.Models;

public record TfSpacePage
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; } = null;
	public Guid SpaceId { get; set; }
	public TfSpacePageType Type { get; set; } = TfSpacePageType.Page;
	public string Name { get; set; }
	public string FluentIconName { get; set; } = TfConstants.PageIconString;
	public short? Position { get; set; }
	public Guid? ComponentId { get; set; }
	public Type ComponentType { get; internal set; }
	public string ComponentOptionsJson { get; set; } = "{}";
	public List<TfSpacePage> ChildPages { get; set; } = new();
	public TfSpacePage? ParentPage { get; set; } = null;

	internal List<TfSpacePage> GetChildPagesPlainList()
	{
		List<TfSpacePage> result = new List<TfSpacePage>();
		Queue<TfSpacePage> queue = new Queue<TfSpacePage>();

		foreach (var page in ChildPages)
			queue.Enqueue(page);

		while (queue.Count > 0)
		{
			var page = queue.Dequeue();

			result.Add(page);

			foreach (var childPage in page.ChildPages)
				queue.Enqueue(childPage);

		}

		return result;
	}
	public override string ToString()
	{
		return $"{Name} (pos:{Position}; par:{ParentPage?.Name})";
	}

	public TfMenuItem ToMenuItem(Action<TfMenuItem>? postProcess = null)
	{
		var item = new TfMenuItem
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(Id),
			Expanded = false,
			IconCollapsed = TfConstants.GetIcon(FluentIconName),
			IconExpanded = TfConstants.GetIcon(FluentIconName),
			Text = Name,
			Items = ChildPages.Select(x => x.ToMenuItem(postProcess)).ToList(),
			OnClick = null,
			OnExpand = null,
			Data = new TfMenuItemData
			{
				MenuType = TfMenuItemType.None,
				SpacePageType = Type,
				SpaceId = SpaceId
			},
			Url = Type == TfSpacePageType.Folder ? null : string.Format(TfConstants.SpaceNodePageUrl, SpaceId, Id),
			Description = null
		};

		if (postProcess is not null)
		{
			postProcess(item);
		}

		return item;
	}
}


[DboCacheModel]
[TfDboModel("tf_space_page")]
public class TfSpacePageDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("parent_id")]
	public Guid? ParentId { get; set; } = null;

	[TfDboModelProperty("space_id")]
	public Guid SpaceId { get; set; }

	[TfDboModelProperty("type")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfSpacePageType>))]
	public TfSpacePageType Type { get; set; }

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


