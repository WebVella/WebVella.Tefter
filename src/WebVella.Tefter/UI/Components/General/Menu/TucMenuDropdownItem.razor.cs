namespace WebVella.Tefter.UI.Components;
public partial class TucMenuDropdownItem : TfBaseComponent
{
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public TfMenuItem Item { get; set; } = new();
	[Parameter] public int? EllipsesLimit { get; set; } = 20;

	private string? _css  = null;
	private IReadOnlyDictionary<string, object>? _attributes = null;
	private string _hash = String.Empty;
	protected override void OnInitialized()
	{
		_init();
	}

	protected override void OnParametersSet()
	{
		var hash = Item.Hash;
		if(_hash != hash){ 
			_hash = hash;
			_init();
			StateHasChanged();
		}
	}

	private void _init()
	{
		_css  = null;

		_attributes = (new Dictionary<string, object>() { { "title", Item.Tooltip ?? String.Empty } }).AsReadOnly();

		var classList = new List<string>();
		classList.Add("tf-tabs__item");
		if (!String.IsNullOrWhiteSpace(Class)) classList.Add(Class);
		if (Item.Selected) classList.Add("tf-tabs__item--active");

		if (Item.Disabled)
			classList.Add("tf-tabs__item--disabled");

		if(Item.Data is not null) 
			classList.Add($"tf-tabs__item--{Item.Data.SpacePageType.ToDescriptionString()}");

		_css = String.Join(" ", classList);
	}
}