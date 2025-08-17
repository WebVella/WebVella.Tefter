namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewPageContentToolbar.TucSpaceViewPageContentToolbar", "WebVella.Tefter")]
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	// Dependency Injection
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	[Parameter] public EventCallback<string> OnSearch { get; set; }
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfSpaceData SpaceData { get; set; } = default!;
	[Parameter] public TfSpaceViewPreset? SpaceViewPreset { get; set; } = null;
	[Parameter] public TfDataTable Data { get; set; } = default!;

	private TfNavigationState _navState = default!;
	private TfUser _currentUser = default!;
	private bool _hasViewPersonalization = false;
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUserUIService.UserUpdated -= On_UserChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);

		await _init();
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfUserUIService.UserUpdated += On_UserChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init();
	}
	private async void On_UserChanged(object? caller, TfUser args)
	{
		await _init();
	}
	private async Task _init()
	{
		_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if (_navState is null) return;
		try
		{
			_currentUser = await TfUserUIService.GetCurrentUserAsync() ?? new();
			var bookmarks = TfUserUIService.GetUserBookmarks(_currentUser.Id);
			var saves = TfUserUIService.GetUserSaves(_currentUser.Id);

			_activeBookmark = bookmarks.FirstOrDefault(x => x.SpaceViewId == _navState.SpaceViewId);
			_activeSavedUrl = saves.FirstOrDefault(x => x.Id == _navState.ActiveSaveId);
			_hasViewPersonalization = _currentUser.Settings.ViewPresetColumnPersonalizations.Any(x =>
				x.SpaceViewId == SpaceView.Id && x.PresetId == SpaceViewPreset?.Id);
			if (!_hasViewPersonalization)
			{
				_hasViewPersonalization = _currentUser.Settings.ViewPresetSortPersonalizations.Any(x => x.SpaceViewId == SpaceView.Id
				 && x.PresetId == SpaceViewPreset?.Id);
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _searchChanged(string value) => await OnSearch.InvokeAsync(value);

	private Task _onAddRowClick()
	{
		if (Data is null) return Task.CompletedTask;
		try
		{
			var newDt = Data.NewTable();
			newDt.Rows.Add(newDt.NewRow());

			var result = TfSpaceDataUIService.SaveDataDataTable(newDt);
			var clone = Data.Clone();
			clone.Rows.Insert(0, result.Rows[0]);
			ToastService.ShowSuccess(LOC("Row added"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}
}