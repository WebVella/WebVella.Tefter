namespace WebVella.Tefter.UI.Components;
public partial class TucMenu : TfBaseComponent
{
	[Parameter] public string? Style { get; set; } = null;
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public string? ItemClass { get; set; } = null;
	[Parameter] public List<TfMenuItem> Items { get; set; } = new();
	[Parameter] public EventCallback<TfMenuItem> OnClick { get; set; }
	[Parameter] public int? EllipsesLimit { get; set; } = 20;
	[Parameter] public int? Limit { get; set; } = null;
	[Parameter] public int Level { get; set; } = 0;
	[Parameter] public Icon? MoreIcon { get; set; } = TfConstants.GetIcon("MoreHorizontal");
	[Parameter] public string? MoreText { get; set; } = null;


	private List<TfMenuItem> _displayItems = new();
	private List<TfMenuItem> _overflowItems = new();
	private TfMenuItem? _moreItem = null;
	private string? _css = null;
	private string? _style = null;
	private string _menuHash = String.Empty;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_init();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		var hash = String.Empty;
		foreach (var item in Items ?? new())
		{
			hash += item.Hash;
		}
		if(_menuHash != hash){ 
			_menuHash = hash;
			_init();
		}
	}

	private void _init()
	{
		_displayItems = new();
		_overflowItems = new();
		_moreItem = null;
		_css = null;
		_style = null;

		//Css
		var classList = new List<string>();
		classList.Add("tf-menu");
		classList.Add($"tf-menu--level-{Level}");
		if (!String.IsNullOrWhiteSpace(Class))
			classList.Add(Class);
		_css = String.Join(" ", classList);

		//Style
		_style = Style;

		//Assign actions
		foreach (var item in Items)
		{
			_initItemActions(item);
		}

		//Overflow
		if (Limit is not null && Items.Count > Limit)
		{
			_displayItems = Items.Take(Limit.Value).ToList();
			_overflowItems = Items.Skip(Limit.Value).ToList();
		}
		else
		{
			_displayItems = Items;
		}

		//More item
		if (_overflowItems.Count > 0)
		{
			_moreItem = new TfMenuItem
			{
				Id = Guid.NewGuid().ToString(),
				IconCollapsed = MoreIcon,
				IconExpanded = MoreIcon,
				Text = MoreText
			};
		}
	}

	private void _initItemActions(TfMenuItem item)
	{
		if (String.IsNullOrWhiteSpace(item.Id))
			item.Id = TfConverters.ConvertGuidToHtmlElementId();
		item.OnClick = async () => await _onClick(item);
		item.OnExpand = async (expand) => await _onExpand(item, expand);
		foreach (var node in item.Items)
		{
			_initItemActions(node);
		}
	}

	private async Task _onClick(TfMenuItem item)
	{
		if (item.ShouldNavigate)
		{
			Navigator.NavigateTo(item.Href!);
		}
		else if(item.Data is not null && OnClick.HasDelegate){ 
			await OnClick.InvokeAsync(item);
		}
		else if (item.Items.Count > 0)
			await _onExpand(item, !item.Expanded);
	}

	private Task _onExpand(TfMenuItem item, bool expand)
	{
		item.Expanded = expand;
		StateHasChanged();
		return Task.CompletedTask;
	}

	private Task _showOverflow()
	{
		return Task.CompletedTask;
	}
}