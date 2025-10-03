namespace WebVella.Tefter.UI.Components;
public partial class TucSelectIcon : TfBaseComponent
{
	[Parameter] public string? Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public string? Placeholder { get; set; }
	[Parameter] public bool Required { get; set; } = false;
	[Parameter] public bool Readonly { get; set; } = false;
	[Parameter] public bool Disabled { get; set; } = false;
	private bool _isReadonly { get => Readonly || !ValueChanged.HasDelegate; }

	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private IconInfo? _icon
	{
		get => String.IsNullOrWhiteSpace(Value) ? null : TfConstants.GetIcon(Value) ; 
	}
	private bool _open = false;
	private FluentSearch? _searchInput;
	private string? _search = null;
	private List<string> _icons
	{
		get
		{
			if (String.IsNullOrWhiteSpace(_search)) return TfConverters.GetSpaceIconLibrary();
			var lowerSearch = _search.Trim().ToLowerInvariant();
			return TfConverters.GetSpaceIconLibrary().Where(x => x.ToLowerInvariant().Contains(lowerSearch)).ToList();
		}
	}

	private async Task _onOpenChanged(bool isOpened)
	{
		_open = isOpened;
		if (!_open)
		{
			_search = null;
		}
		else
		{
			await InvokeAsync(StateHasChanged);
			await Task.Delay(400);
			_searchInput?.FocusAsync();
		}
	}

	private async Task _optionChanged(string? option)
	{
		await ValueChanged.InvokeAsync(option);
		_open = false;
	}
}