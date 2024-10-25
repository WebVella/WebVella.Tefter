namespace WebVella.Tefter.Web.Components;
public partial class TfSelectIcon : TfBaseComponent
{
	[Parameter] public string Value { get; set; }
	[Parameter] public EventCallback<string> ValueChanged { get; set; }
	[Parameter] public string Placeholder { get; set; }
	private bool _isReadonly { get => !ValueChanged.HasDelegate; }

	private string _elementId = TfConverters.ConvertGuidToHtmlElementId(Guid.NewGuid());
	private IconInfo _icon
	{
		get => TfConstants.GetIcon(Value);
	}
	private bool _open = false;
	private FluentSearch _searchInput;
	private string _search = null;
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

	private async Task _optionChanged(string option)
	{
		await ValueChanged.InvokeAsync(option);
		_open = false;
	}
}