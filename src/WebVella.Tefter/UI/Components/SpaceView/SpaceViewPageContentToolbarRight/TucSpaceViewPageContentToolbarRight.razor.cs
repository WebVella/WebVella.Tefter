using DocumentFormat.OpenXml.Office.CustomUI;

namespace WebVella.Tefter.UI.Components;

public partial class TucSpaceViewPageContentToolbarRight : TfBaseComponent
{
	// Dependency Injection
	[CascadingParameter(Name = "TucSpaceViewPageContent")]
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;
	[Parameter] public TfSpacePageAddonContext Context { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;
	[Parameter] public TfSpaceViewPreset? SpaceViewPreset { get; set; } = null;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public EventCallback<TfDataTable> DataChanged { get; set; }
	[Parameter] public bool SelectAllLoading { get; set; } = false;
	[Parameter] public List<Guid> SelectedRows { get; set; } = new();

	private TfNavigationState _navState = null!;
	private bool _hasViewPersonalization = false;

	public void Dispose()
	{
		TfAuthLayout.NavigationStateChangedEvent -= On_NavigationStateChanged;
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = TfAuthLayout.NavigationState;
		await _init(_navState);

		TfAuthLayout.NavigationStateChangedEvent += On_NavigationStateChanged;
		TfEventProvider.UserUpdatedGlobalEvent += On_UserChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}
	private async void On_UserChanged(TfUserUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (Context is not null)
				Context.CurrentUser = args.Payload;
			await _init(TfAuthLayout.NavigationState);
		});
	}
	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
			var bookmarks = TfService.GetBookmarksListForUser(Context.CurrentUser.Id);
			var saves = TfService.GetSavesListForUser(Context.CurrentUser.Id);

			_activeBookmark = bookmarks.FirstOrDefault(x => x.SpaceViewId == _navState.SpaceViewId);
			_activeSavedUrl = saves.FirstOrDefault(x => x.Id == _navState.ActiveSaveId);
			_hasViewPersonalization = Context.CurrentUser.Settings.ViewPresetColumnPersonalizations.Any(x =>
				x.SpaceViewId == SpaceView.Id && x.PresetId == SpaceViewPreset?.Id);
			if (!_hasViewPersonalization)
			{
				_hasViewPersonalization = Context.CurrentUser.Settings.ViewPresetSortPersonalizations.Any(x => x.SpaceViewId == SpaceView.Id
				 && x.PresetId == SpaceViewPreset?.Id);
			}
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _searchChanged(string value) => await TucSpaceViewPageContent.OnSearch(value);

	private Task _onAddRowClick()
	{
		if (Data is null) return Task.CompletedTask;
		try
		{
			var result = TfService.InsertRowInDataTable(Data);
			TucSpaceViewPageContent.OnNewRow(result);
			ToastService.ShowSuccess(LOC("Row added"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		return Task.CompletedTask;
	}

	private void _onEditAllClick()
	{
		TucSpaceViewPageContent.ToggleEditAll();
	}
}