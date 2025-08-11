namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewSortsDialog.TucSpaceViewSortsDialog", "WebVella.Tefter")]
public partial class TucSpaceViewSortsDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public Guid Content { get; set; } = default!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private TfDataProvider? _dataProvider = null;
	private TfSpaceData? _spaceData = null;
	private List<TfSort> _items = new List<TfSort>();
	private List<string> _columnOptions = new List<string>();

	private TfSort _selectedSort = new();
	private string _activeTab = "current";
	private TfNavigationState _navState = default!;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		if (_navState is null || Content == Guid.Empty)
			throw new Exception("SpaceViewId not found");
		if (_navState.Sorts is not null)
			_items = JsonSerializer.Deserialize<List<TfSort>>(JsonSerializer.Serialize(_navState.Sorts)) ?? new();
		var spaceView = TfSpaceViewUIService.GetSpaceView(Content);
		if (spaceView is null)
			throw new Exception("spaceView not found");

		_spaceData = TfSpaceDataUIService.GetSpaceData(spaceView.SpaceDataId);

		if (_spaceData is not null)
		{
			_dataProvider = TfDataProviderUIService.GetDataProvider(_spaceData.DataProviderId);
		}
		_generateColumnOptions();
	}


	private async Task _submit()
	{
		await Dialog.CloseAsync(_items);
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private void _generateColumnOptions()
	{
		_columnOptions = _spaceData.Columns.Where(x => !_items.Any(y => y.ColumnName == x)).ToList();
	}

	private void _addSortColumn()
	{
		_items.Add(_selectedSort with { ColumnName = _selectedSort.ColumnName });
		_selectedSort = new();
	}

	private void _deleteSortColumn(TfSort tucSort)
	{
		_items = _items.Where(x => x.ColumnName != tucSort.ColumnName).ToList();
	}

}
