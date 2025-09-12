namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewFiltersDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Inject] public ITfSharedColumnUIService TfSharedColumnUIService { get; set; } = default!;
	[Parameter] public Guid Content { get; set; } = default!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private List<TfDataProvider> _dataProviders = new();
	private List<TfSharedColumn> _sharedColumns = new();
	private TfDataProvider? _dataProvider = null;
	private TfSpaceData? _spaceData = null;
	private TfSpaceView? _spaceview = null;
	private List<TfSpaceViewColumn> _viewColumns = new();
	private List<TfFilterQuery> _items = new List<TfFilterQuery>();
	private string _activeTab = "current";
	internal string? _selectedFilterColumn = null;
	public bool _submitting = false;
	private TfNavigationState _navState = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if(_navState is null || Content == Guid.Empty)
			throw new Exception("SpaceViewId not found");
		_spaceview = TfSpaceViewUIService.GetSpaceView(Content);
		if(_spaceview is null)
			throw new Exception("spaceView not found");
		_viewColumns = TfSpaceViewUIService.GetViewColumns(Content);
		_spaceData =TfSpaceDataUIService.GetSpaceData(_spaceview.SpaceDataId);
		_dataProviders = TfDataProviderUIService.GetDataProviders().ToList();
		_sharedColumns = TfSharedColumnUIService.GetSharedColumns();
		if (_spaceData is not null)
		{
			_dataProvider = _dataProviders.FirstOrDefault(x=> x.Id == _spaceData.DataProviderId);
		}
		_items = _navState?.Filters ?? new();
	}

	private Task _onFiltersChangeHandler(List<TfFilterQuery> filters)
	{
		_items = filters;
		return Task.CompletedTask;
	}
	private async Task _submit()
	{
		await Dialog.CloseAsync(_items);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
