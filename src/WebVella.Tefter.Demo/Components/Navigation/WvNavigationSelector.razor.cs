
namespace WebVella.Tefter.Demo.Components;
public partial class WvNavigationSelector : WvBaseComponent
{
	[Parameter]
	public Guid? SpaceId { get; set; } = null;

	[Parameter]
	public EventCallback<Space> SpaceChanged { get; set; }


	private bool _open = false;
	private bool _selectorLoading = true;
	private List<Space> _spaces = null;
	private List<Space> _selectorItems = new();
	private Space _selectedSpace = new();
	private string _selectorSearch = "";
	private int _optionsCountLimit = 10;

	private FluentSearch _searchField;

	private void _init()
	{
		if (_spaces is null)
		{
			_spaces = WvState.GetSpaces();
			foreach (var space in _spaces)
			{
				space.OnSelect = async ()=> await _spaceSelected(space);
			}
		}
		_selectorSearch = "";
		_selectorItems = _spaces.ToList();
	}

	public async Task ToggleSelector()
	{
		_open = !_open;
		if (_open)
		{
			_selectorLoading = true;
			await InvokeAsync(StateHasChanged);
			_init();

			_selectorLoading = false;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(100);
			_searchField!.FocusAsync();
		}
	}

	private void _searchValueChanged(string search)
	{
		_selectorSearch = search;
		_selectorItems.Clear();
		if(String.IsNullOrWhiteSpace(_selectorSearch)){ 
			_selectorItems = _spaces.ToList();
		}
		else{ 
			var lowerSearch = _selectorSearch.Trim().ToLowerInvariant();
			_selectorItems = _spaces.Where(x=> x.Name.ToLowerInvariant().Contains(lowerSearch)).ToList();
		}
	}

	private async Task _spaceSelected(Space space)
	{
		await SpaceChanged.InvokeAsync(space);
		_open = false;

	}

	private async Task _browserAll() {
		ToastService.ShowToast(ToastIntent.Warning, "Will show a page with the list of all workspaces with ability to browser");
	}
}