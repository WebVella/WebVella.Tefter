namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarLeft : TfBaseComponent
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

	private TfNavigationState _navState = null!;
	private bool _hasViewPersonalization = false;
	public void Dispose()
	{
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfEventProvider.UserUpdatedGlobalEvent -= On_UserChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = TfAuthLayout.NavigationState;
		await _init(_navState);
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
		TfEventProvider.UserUpdatedGlobalEvent += On_UserChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}
	private async void On_UserChanged(TfUserUpdatedEvent args)
	{
		if(Context is not null)
			Context.CurrentUser = args.Payload;
		await _init(TfAuthLayout.NavigationState);
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
			var result = TfUIService.InsertRowInDataTable(Data);
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