namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewSortsDialog.TfSpaceViewSortsDialog", "WebVella.Tefter")]
public partial class TfSpaceViewSortsDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private TucSpaceData _spaceData = null;
	private TucDataProvider _dataProvider = null;
	private List<TucSort> _items = new List<TucSort>();
	private List<string> _columnOptions = new List<string>();

	private TucSort _selectedSort = new();
	private string _activeTab = "current";

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.SpaceViewSorts is not null)
			_items = JsonSerializer.Deserialize<List<TucSort>>(JsonSerializer.Serialize(TfAppState.Value.SpaceViewSorts));
		_spaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);
		if (_spaceData is not null)
		{
			_dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);
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
		_columnOptions = _spaceData.Columns.Where(x => !_items.Any(y => y.DbName == x)).ToList();
	}

	private void _addSortColumn()
	{
		_items.Add(_selectedSort with { DbName = _selectedSort.DbName });
		_selectedSort = new();
	}

	private void _deleteSortColumn(TucSort tucSort)
	{
		_items = _items.Where(x => x.DbName != tucSort.DbName).ToList();
	}

}
