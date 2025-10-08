namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewFiltersDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Parameter] public Guid Content { get; set; } = Guid.Empty;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;
	private List<TfDataProvider> _dataProviders = new();
	private List<TfSharedColumn> _sharedColumns = new();
	private TfDataProvider? _dataProvider = null;
	private TfDataset? _spaceData = null;
	private TfSpaceView? _spaceview = null;
	private List<TfSpaceViewColumn> _viewColumns = new();
	private List<TfFilterQuery> _items = new List<TfFilterQuery>();
	private string _activeTab = "current";
	internal string? _selectedFilterColumn = null;
	public bool _submitting = false;
	private TfNavigationState _navState = null!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = TfState.NavigationState;
		if(_navState is null || Content == Guid.Empty)
			throw new Exception("SpaceViewId not found");
		_spaceview = TfService.GetSpaceView(Content);
		if(_spaceview is null)
			throw new Exception("spaceView not found");
		_viewColumns = TfService.GetSpaceViewColumnsList(Content);
		_spaceData = TfService.GetDataset(_spaceview.DatasetId);
		_dataProviders = TfService.GetDataProviders().ToList();
		_sharedColumns = TfService.GetSharedColumns();
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
