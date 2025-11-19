namespace WebVella.Tefter.Models;

public record TfMenuItem
{
	public string? Id { get; set; } = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	public string? Text { get; set; }
	public string? Tooltip { get; set; }
	public string? Description { get; set; }
	public string? Abbriviation { get; set; }
	public bool Disabled { get; set; } = false;
	public bool Completed { get; set; } = false;
	public Icon? IconCollapsed { get; set; }
	public Icon? IconExpanded { get; set; }
	public TfColor? Color { get; set; }
	public TfColor? IconColor { get; set; }
	public bool SpinIcon { get; set; } = false;

	public Icon? Icon
	{
		get
		{
			return Expanded
				? (IconExpanded is null || IconColor is null
					? IconExpanded
					: IconExpanded?.WithColor(IconColor.GetColor().OKLCH))
				: (IconCollapsed is null || IconColor is null
					? IconCollapsed
					: IconCollapsed?.WithColor(IconColor.GetColor().OKLCH));
		}
	}

	public string? Url { get; set; }

	public string? Href
	{
		get
		{
			if (Disabled) return null;
			if (String.IsNullOrWhiteSpace(Url)) return "#";
			return Url;
		}
	}

	public bool? IsSeparateChevron
	{
		get
		{
			if (Items.Count == 0) return null;
			if (!ShouldNavigate) return false;
			return true;
		}
	}

	public bool ShouldNavigate
	{
		get
		{
			if (Href is null) return false;
			if (Href == "#") return false;
			return true;
		}
	}

	public bool Expanded { get; set; } = false;
	public bool Selected { get; set; } = false;

	public bool ChildSelected
	{
		get
		{
			foreach (var item in Items)
			{
				if (item.Selected || item.ChildSelected) return true;
			}
			return false;
		}
	}

	public RenderFragment? BadgeContent { get; set; } = null;

	public string IdTree
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(Id);
			// if (String.IsNullOrWhiteSpace(Url))
			// {
				foreach (var item in Items)
				{
					sb.Append(item.IdTree);
				}
			//}

			return sb.ToString();
		}
	}

	public TfMenuItemData? Data { get; set; } = null;
	public List<TfMenuItem> Items { get; set; } = new();
	public List<TfMenuItem> Actions { get; set; } = new();
	public ElementReference Reference { get; set; }

	public string Hash
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(
				$"{Id}{Url}{Text}{Description}{Expanded}{Selected}{IconCollapsed?.Name}{IconExpanded?.Name}{(int?)Color}{(int?)IconColor}{OnClick.HasDelegate}{OnExpand.HasDelegate}");
			foreach (var item in Items)
			{
				sb.Append(item.Hash);
			}

			return sb.ToString();
		}
	}

	[JsonIgnore] public EventCallback OnClick { get; set; }

	[JsonIgnore] public EventCallback<bool> OnExpand { get; set; }
	[JsonIgnore] public EventCallback OnActions { get; set; }
}

public record TfMenuItemData
{
	public Guid? SpaceId { get; set; }
	public TfMenuItemType MenuType { get; set; } = TfMenuItemType.None;
	public TfSpacePageType SpacePageType { get; set; } = TfSpacePageType.Page;
	public TfSpacePageCreateFromFileContextItem? ImportFileContext { get; set; } = null;
}

public enum TfMenuItemType
{
	None = 0,
	CreateSpace = 1,
	CreateSpacePage = 2,
	DeleteSpace = 3,
}