namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewPageContentToolbarLeft : TfBaseComponent
{
	// Dependency Injection
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfUserUIService TfUserUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	[CascadingParameter(Name = "TucSpaceViewPageContent")] 
	public TucSpaceViewPageContent TucSpaceViewPageContent { get; set; } = default!;
	[Parameter] public TfSpacePageAddonContext Context { get; set; } = default!;
	[Parameter] public TfSpaceView SpaceView { get; set; } = default!;
	[Parameter] public TfDataSet SpaceData { get; set; } = default!;
	[Parameter] public TfSpaceViewPreset? SpaceViewPreset { get; set; } = null;
	[Parameter] public TfDataTable Data { get; set; } = default!;
	[Parameter] public EventCallback<TfDataTable> DataChanged { get; set; }

	private TfNavigationState _navState = default!;
	private bool _hasViewPersonalization = false;
	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
		TfUserUIService.UserUpdated -= On_UserChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = Navigator.GetRouteState();
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
		if(Context is not null)
			Context.CurrentUser = args;
		await _init();
	}
	private async Task _init()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if (navState is null) return;
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
			var result = TfSpaceDataUIService.InsertRowInDataTable(Data);
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