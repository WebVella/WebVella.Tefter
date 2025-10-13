namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbar : TfBaseComponent
{
	// Dependency Injection
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
	[CascadingParameter(Name = "TucSpaceViewPageContent")] 
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = null!;
	[Parameter] public TfSpacePageAddonContext Context { get; set; } = null!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = null!;
	[Parameter] public TfDataset SpaceData { get; set; } = null!;
	[Parameter] public TfSpaceViewPreset? SpaceViewPreset { get; set; } = null;
	[Parameter] public TfDataTable Data { get; set; } = null!;
	[Parameter] public EventCallback<TfDataTable> DataChanged { get; set; }
	
	private TfNavigationState _navState = null!;
	private bool _hasViewPersonalization = false;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider?.Dispose();
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = TfAuthLayout.GetState().NavigationState;
		await _init(_navState);
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.UserUpdatedEvent += On_UserChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			if (UriInitialized != args.Location)
				await _init(TfAuthLayout.GetState().NavigationState);
		});
	}
	private async Task On_UserChanged(TfUserUpdatedEvent args)
	{
		await InvokeAsync(async () =>
		{
			if (Context is not null)
				Context.CurrentUser = args.Payload;
			await _init(TfAuthLayout.GetState().NavigationState);
		});
	}
	private async Task _init(TfNavigationState navState)
	{
		_navState = navState;
		try
		{
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