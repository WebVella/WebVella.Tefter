namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceDataDetailsContent : TfBaseComponent, IDisposable
{
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSpaceUIService TfSpaceUIService { get; set; } = default!;
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private TfSpaceData _spaceData = new();
	private TfSpace _space = new();
	private TfDataProvider _provider = new();
	public bool _submitting = false;
	public TfNavigationState? _navState = null;
	private List<TfSpaceDataColumn> _spaceDataColumns = new();
	private List<TfSpaceDataColumn> _columnOptions = new();
	public void Dispose()
	{
		TfSpaceDataUIService.SpaceDataUpdated -= On_SpaceDataUpdated;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init();
		TfSpaceDataUIService.SpaceDataUpdated += On_SpaceDataUpdated;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_SpaceDataUpdated(object? caller, TfSpaceData args)
	{
		await _init(spaceData: args);
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}

	private async Task _init(TfNavigationState? navState = null, TfSpaceData? spaceData = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			if (spaceData is not null && spaceData.Id == _spaceData?.Id)
			{
				_spaceData = spaceData;
			}
			else
			{
				var routeData = Navigator.GetRouteState();
				if (routeData.SpaceDataId is not null)
					_spaceData = TfSpaceDataUIService.GetSpaceData(routeData.SpaceDataId.Value);

			}
			if (_spaceData is null) return;
			_space = TfSpaceUIService.GetSpace(_spaceData.SpaceId);
			_provider = TfDataProviderUIService.GetDataProvider(_spaceData.DataProviderId);
			_spaceDataColumns = TfSpaceDataUIService.GetSpaceDataColumns(_spaceData.Id);
			_columnOptions = TfSpaceDataUIService.GetSpaceDataColumnOptions(_spaceData.Id);

		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _editSpaceData()
	{
		var dialog = await DialogService.ShowDialogAsync<TucSpaceDataManageDialog>(
		_spaceData,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null) { }
	}

	private async Task _deleteSpaceData()
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space data deleted?")))
			return;
		try
		{
			TfSpaceDataUIService.DeleteSpaceData(_spaceData.Id);
			var allSpaceData = TfSpaceDataUIService.GetSpaceDataList(_navState!.SpaceId!.Value);
			ToastService.ShowSuccess(LOC("Space data removed"));
			if (allSpaceData.Count > 0)
			{
				Navigator.NavigateTo(string.Format(TfConstants.SpaceDataPageUrl, _navState.SpaceId, allSpaceData[0].Id));
			}
			else{ 
				Navigator.NavigateTo(TfConstants.HomePageUrl);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}

	private void _copySpaceData()
	{
		try
		{
			TfSpaceDataUIService.CopySpaceData(_spaceData.Id);
			ToastService.ShowSuccess(LOC("Space copied"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}


}