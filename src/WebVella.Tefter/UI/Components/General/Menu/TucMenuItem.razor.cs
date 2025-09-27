namespace WebVella.Tefter.UI.Components;

public partial class TucMenuItem : TfBaseComponent
{
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public TfMenuItem Item { get; set; } = new();
	[Parameter] public int? EllipsesLimit { get; set; } = 20;

	private IReadOnlyDictionary<string, object>? _attributes = null;
	private TfMenuItem? _expandItem = null;
	private string? _css = null;
	private string? _expandItemCss = null;
	private string _hash = String.Empty;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_init();
	}

	protected override void OnParametersSet()
	{
		var hash = Item.Hash;
		if (_hash != hash)
		{
			_hash = hash;
			_init();
		}
	}

	private void _init()
	{
		_expandItem = null;
		_css = null;
		_expandItemCss = null;

		_attributes = (new Dictionary<string, object>() { { "title", Item.Tooltip ?? String.Empty } }).AsReadOnly();

		var classList = new List<string>();
		classList.Add("tf-menu__item");
		if (!String.IsNullOrWhiteSpace(Class)) classList.Add(Class);
		if (Item.Selected) classList.Add("tf-menu__item--active");

		if (Item.Disabled)
			classList.Add("tf-menu__item--disabled");

		if (Item.Data is not null)
			classList.Add($"tf-menu__item--{Item.Data.SpacePageType.ToDescriptionString()}");

		_css = String.Join(" ", classList);

		var expandClassList = classList.ToList();
		expandClassList.Add("tf-menu__item--expand");
		_expandItemCss = String.Join(" ", expandClassList);

		if (Item.IsSeparateChevron is not null
		&& Item.IsSeparateChevron.Value)
		{
			_expandItem = new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				Text = null,
				IconCollapsed = TfConstants.GetIcon("ChevronDown"),
				IconExpanded = TfConstants.GetIcon("ChevronDown"),
				OnClick = EventCallback.Factory.Create(this, async () => await Item.OnExpand.InvokeAsync(!Item.Expanded)),
			};

		}

	}

}