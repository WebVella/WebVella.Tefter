namespace WebVella.Tefter.Models;

public record TfSpacePage
{
	public Guid Id { get; set; }
	public Guid? ParentId { get; set; } = null;
	public Guid SpaceId { get; set; }
	public TfSpacePageType Type { get; set; } = TfSpacePageType.Page;
	public string Name { get; set; }
	public string FluentIconName { get; set; } = "Document";
	public string? Description { get; set; }
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

	public Guid? GetFirstNavigatedPageId()
	{
		if (Type != TfSpacePageType.Folder)
			return Id;
		foreach (var child in ChildPages ?? new List<TfSpacePage>())
		{
			var childId = child.GetFirstNavigatedPageId();
			if (childId is not null)
				return childId;
		}
		return null;
	}

	[Obsolete]
	public TfMenuItem ToMenuItem(Action<TfMenuItem>? postProcess = null, Func<TfSpacePage, bool>? includeChildFunc = null)
	{
		var item = new TfMenuItem
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(Id),
			Expanded = false,
			IconCollapsed = TfConstants.GetIcon(FluentIconName),
			IconExpanded = TfConstants.GetIcon(FluentIconName),
			Text = Name,
			Items = new(),
			Data = new TfMenuItemData
			{
				MenuType = TfMenuItemType.None,
				SpacePageType = Type,
				SpaceId = SpaceId
			},
			Url = Type == TfSpacePageType.Folder ? null : string.Format(TfConstants.SpacePagePageUrl, SpaceId, Id),
			Description = null
		};

		foreach (var childPage in ChildPages)
		{
			if (includeChildFunc is not null && !includeChildFunc(childPage))
				continue;
			var childItem = childPage.ToMenuItem(postProcess, includeChildFunc);
			item.Items.Add(childItem);
		}

		if (postProcess is not null)
		{
			postProcess(item);
		}

		return item;
	}

	public Guid? TemplateId { get; set; } = null;
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
	public string? Icon { get; set; } = null;

	[TfDboModelProperty("description")] 
	public string? Description { get; set; } = null;	
	
	[TfDboModelProperty("position")]
	public short Position { get; set; }

	[TfDboModelProperty("component_id")]
	public Guid? ComponentId { get; set; }

	[TfDboModelProperty("component_settings_json")]
	public string ComponentSettingsJson { get; set; } = "{}";
}

[DboCacheModel]
[TfDboModel("tf_space_page_tag")]
internal class TfSpacePageTag
{
	[TfDboModelProperty("space_page_id")] public Guid SpacePageId { get; set; }

	[TfDboModelProperty("tag_id")] public Guid TagId { get; set; }
}
