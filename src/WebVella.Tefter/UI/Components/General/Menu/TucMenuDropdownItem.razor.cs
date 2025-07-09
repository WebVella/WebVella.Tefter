namespace WebVella.Tefter.UI.Components;
public partial class TucMenuDropdownItem : TfBaseComponent
{
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public TfMenuItem Item { get; set; } = new();
	[Parameter] public int? EllipsesLimit { get; set; } = 20;

	private string? _css  = null;
	private IReadOnlyDictionary<string, object>? _attributes = null;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_init();
	}

	private void _init()
	{
		_css  = null;

		_attributes = (new Dictionary<string, object>() { { "title", Item.Tooltip ?? String.Empty } }).AsReadOnly();

		var classList = new List<string>();
		classList.Add("tf-menu__item");
		if (!String.IsNullOrWhiteSpace(Class)) classList.Add(Class);
		if (Item.Selected) classList.Add("tf-menu__item--active");

		if (Item.Disabled)
			classList.Add("tf-menu__item--disabled");
		_css = String.Join(" ", classList);
	}
}