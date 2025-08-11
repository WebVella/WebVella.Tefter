namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewFiltersDialog.TucSpaceViewFiltersDialog", "WebVella.Tefter")]
public partial class TucSpaceViewFiltersDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Inject] public ITfDataProviderUIService TfDataProviderUIService { get; set; } = default!;
	[Parameter] public Guid Content { get; set; } = default!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
	private TfDataProvider? _dataProvider = null;
	private TfSpaceData? _spaceData = null;
	private List<TfFilterBase> _items = new List<TfFilterBase>();
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
		if (_navState.Filters is not null)
			_items = JsonSerializer.Deserialize<List<TfFilterBase>>(JsonSerializer.Serialize(_navState.Filters)) ?? new();
		var spaceView = TfSpaceViewUIService.GetSpaceView(Content);
		if(spaceView is null)
			throw new Exception("spaceView not found");

		_spaceData =TfSpaceDataUIService.GetSpaceData(spaceView.SpaceDataId);

		if (_spaceData is not null)
		{
			_dataProvider = TfDataProviderUIService.GetDataProvider(_spaceData.DataProviderId);
		}
	}

	private Task _onFiltersChangeHandler(List<TfFilterBase> filters)
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
