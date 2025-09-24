namespace WebVella.Tefter.UI.Components;
public partial class TucAsideMenu : TfBaseComponent
{
	[Parameter] public string? Style { get; set; } = null;
	[Parameter] public string? Class { get; set; } = null;
	[Parameter] public string? ItemClass { get; set; } = null;
	[Parameter] public List<TfMenuItem> Items { get; set; } = new();
	[Parameter] public EventCallback<TfMenuItem> OnClick { get; set; }
	[Parameter] public int? EllipsesLimit { get; set; } = 20;
	[Parameter] public int? Limit { get; set; } = null;

	private string? _css = null;
	private string? _style = null;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_init();
	}

	private void _init()
	{
		_css = null;
		_style = null;

		//Css
		var classList = new List<string>();
		classList.Add("tf-asidenav");
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
	}

	private void _initItemActions(TfMenuItem item)
	{
		item.OnClick = EventCallback.Factory.Create(this, async () => await _onClick(item));
		item.OnExpand = EventCallback.Factory.Create<bool>(this, async (expand) => await _onExpand(item, expand));
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
		else if (item.Items.Count > 0)
			await _onExpand(item, !item.Expanded);
	}

	private Task _onExpand(TfMenuItem item, bool expand)
	{
		item.Expanded = expand;
		StateHasChanged();
		return Task.CompletedTask;
	}

	private string _getBadgeStyle(TfMenuItem item)
	{
		if (item.IconColor is null)
			return String.Empty;

		return $"background-color:var({item.IconColor!.GetColor().Variable})";
	}

	private string _getAbbriviationStyle(TfMenuItem item)
	{
		if (item.IconColor is null)
			return String.Empty;

		var color = item.IconColor!.GetColor();
		if(color.UseWhiteForeground)
			return $"color:var(--tf-white)";

		return String.Empty;
	}

	private string _getIconStyle(TfMenuItem item)
	{
		if (item.IconColor is null)
			return String.Empty;

		var color = item.IconColor!.GetColor();
		if(color.UseWhiteForeground)
			return $"color:var(--tf-white)";

		return String.Empty;
	}
}