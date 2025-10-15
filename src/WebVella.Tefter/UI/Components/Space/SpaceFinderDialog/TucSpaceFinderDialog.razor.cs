using Bogus;
using Microsoft.AspNetCore.Components.Web;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceFinderDialog : TfBaseComponent, IDialogContentComponent<TfUser?>, IAsyncDisposable
{
	[Parameter] public TfUser? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string? _search = null;
	private List<TfSpace> _allSpaces = new();
	private HashSet<Guid> _userBookmarks = new();
	private List<TfSpaceFinderItem> _options = new();
	private FluentSearch _searchInput = null!;
	private int _selectedIndex = 0;
	private DotNetObjectReference<TucSpaceFinderDialog> _objectRef = null!;

	public async ValueTask DisposeAsync()
	{
		try
		{
			await JSRuntime.InvokeAsync<object>("Tefter.removeArrowsKeyListener", ComponentId.ToString());
			await JSRuntime.InvokeAsync<object>("Tefter.removeEnterKeyListener", ComponentId.ToString());
		}
		catch
		{
			//In rare ocasions the item is disposed after the JSRuntime is no longer avaible
		}

		_objectRef?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_allSpaces = TfService.GetSpacesListForUser(Content.Id);
		_userBookmarks = TfAuthLayout.GetState().UserBookmarks.Where(x => String.IsNullOrWhiteSpace(x.Url)).Select(x => x.SpaceId)
			.ToHashSet();
		_objectRef = DotNetObjectReference.Create(this);
		_init(null);
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await Task.Delay(1);
			_searchInput?.FocusAsync();
			await JSRuntime.InvokeAsync<object>(
				"Tefter.addArrowsKeyListener", _objectRef, ComponentId.ToString(), "OnArrowHandler");
			await JSRuntime.InvokeAsync<object>(
				"Tefter.addEnterKeyListener", _objectRef, ComponentId.ToString(), "OnEnterKeyHandler");
		}
	}

	private void _init(string? search)
	{
		_options = new();
		_selectedIndex = 0;

		search = search?.Trim().ToLowerInvariant();
		
		foreach (var space in _allSpaces)
		{
			if (!String.IsNullOrWhiteSpace(search) && !space.Name!.ToLowerInvariant().Contains(search)) continue;
			_options.Add(new TfSpaceFinderItem(
				space:space,
				index:0,
				bookmarked:_userBookmarks.Contains(space.Id),
				color: space.Color?.GetColor().OKLCH ?? "var(--tf-accent-color)",
				onClick:null!,
				onBookmark:null!
			));
		}

		_options = _options.OrderBy(x=> x.Bookmarked).ThenBy(x=> x.Space.Name).ToList();
		var index = 0;
		foreach (var option in _options)
		{
			option.Index = index;
			option.OnClick = async void () => await _selectSpace(option);
			option.OnBookmark = async void () => await _bookmarkSpace(option);
			index++;
		}
	}

	private void _searchChanged(string? search)
	{
		_search = search;
		_init(search);
	}

	private async Task _adminNavigation()
	{
		Navigator.NavigateTo(TfConstants.AdminDashboardUrl);
		await Dialog.CloseAsync();
	}

	private async Task _selectSpace(TfSpaceFinderItem option)
	{
		var spacePages = TfService.GetSpacePages(option.Space.Id);
		if (spacePages.Count > 0)
		{
			Navigator.NavigateTo(String.Format(TfConstants.SpacePagePageUrl,option.Space.Id, spacePages[0].Id),false);
			await Dialog.CloseAsync();
			return;
		}
		ToastService.ShowWarning(LOC("Space has no pages created yet"));
	}

	private Task _bookmarkSpace(TfSpaceFinderItem option)
	{
		try
		{
			TfService.ToggleBookmark(
				userId: TfAuthLayout.GetState().User.Id,
				spaceId: option.Space.Id
			);
			option.Bookmarked = !option.Bookmarked;
			if (option.Bookmarked)
				_userBookmarks.Add(option.Space.Id);
			else
				_userBookmarks.Remove(option.Space.Id);
			
			ToastService.ShowSuccess(option.Bookmarked ? LOC("Space Bookmarked") : LOC("Space bookmark removed"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}		

		return Task.CompletedTask;;
	}

	[JSInvokable("OnArrowHandler")]
	public async Task OnArrowHandler(bool isUp)
	{
		if (!isUp)
			_selectedIndex++;
		else
			_selectedIndex--;
		if (_selectedIndex < 0)
			_selectedIndex = 0;
		if(_selectedIndex > _options.Count - 1)
			_selectedIndex = _options.Count - 1;
		
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1); // Give time for rendering to complete
		try
		{
			await JSRuntime.InvokeVoidAsync("Tefter.scrollToElement", $"space-option-{_selectedIndex}");
		}
		catch
		{
			// Ignore errors if element is not found or JS is not available
		}
	}
	
	[JSInvokable("OnEnterKeyHandler")]
	public async Task OnEnterKeyHandler()
	{
		await _selectSpace(_options[_selectedIndex]);
	}	
}