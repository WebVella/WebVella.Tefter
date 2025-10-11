using Bogus;
using Microsoft.AspNetCore.Components.Web;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceFinderDialog : TfBaseComponent, IDialogContentComponent<TfUser?>, IAsyncDisposable
{
	[Parameter] public TfUser? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private string? _search = null;
	private List<TfSpace> _allSpaces = new();
	private List<TfSelectOption> _bookmarkedSpaces = new();
	private List<TfSelectOption> _otherSpaces = new();
	private List<TfSelectOption> _options = new();
	private FluentSearch _searchInput;
	private int _selectedIndex = 0;
	private DotNetObjectReference<TucSpaceFinderDialog> _objectRef;

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
		_objectRef = DotNetObjectReference.Create(this);
		var faker = new Faker("en");
		for (int i = 0; i < 200; i++)
		{
			_allSpaces.Add(new TfSpace()
			{
				Id = Guid.NewGuid(),
				Name = faker.Lorem.Sentence(5),
				FluentIconName = "Settings",
				Color = TfColor.Amber500
			});
		}

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
		_bookmarkedSpaces = new();
		_otherSpaces = new();
		_options = new();
		_selectedIndex = 0;
		var state = TfAuthLayout.GetState();
		var userBookmarks = state.UserBookmarks.Where(x => String.IsNullOrWhiteSpace(x.Url)).Select(x => x.SpaceId)
			.ToHashSet();

		search = search?.Trim().ToLowerInvariant();
		foreach (var space in _allSpaces)
		{
			if (!String.IsNullOrWhiteSpace(search) && !space.Name!.ToLowerInvariant().Contains(search)) continue;
			var option = new TfSelectOption()
			{
				Id = space.Id,
				Label = space.Name!,
				Color = space.Color?.GetColor().Value ?? "var(--tf-accent-color)",
				IconName = space.FluentIconName ?? "Globe",
				OnClick = async void () => await _selectSpace(space.Id),
			};
			if (userBookmarks.Contains(space.Id))
				_bookmarkedSpaces.Add(option);
			else
				_otherSpaces.Add(option);
		}

		_bookmarkedSpaces = _bookmarkedSpaces.OrderBy(x => x.Label).ToList();
		_otherSpaces = _otherSpaces.OrderBy(x => x.Label).ToList();
		_options.AddRange(_bookmarkedSpaces);
		_options.AddRange(_otherSpaces);
	}

	private void _searchChanged(string? search)
	{
		_search = search;
		_init(search);
	}

	private Task _selectSpace(Guid spaceId)
	{
		ToastService.ShowInfo(spaceId.ToString());
		return Task.CompletedTask;;
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
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
		ToastService.ShowInfo(_selectedIndex.ToString());
	}	
}