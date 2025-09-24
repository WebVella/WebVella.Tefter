namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewSortsDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	[Inject] public ITfSpaceViewUIService TfSpaceViewUIService { get; set; } = default!;
	[Inject] public ITfSpaceDataUIService TfSpaceDataUIService { get; set; } = default!;
	[Parameter] public Guid Content { get; set; } = default!;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private TfDataSet _spaceData = null;
	private List<TfSpaceViewColumn> _viewColumns = new();
	private List<TfSortQuery> _items = new();

	private string _activeTab = "current";
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		if (Content == Guid.Empty)
			throw new Exception("SpaceViewId not found");
		var view = TfSpaceViewUIService.GetSpaceView(Content);
		if (view is null)
			throw new Exception("SpaceView not found");

		_viewColumns = TfSpaceViewUIService.GetViewColumns(Content);
		_spaceData = TfSpaceDataUIService.GetSpaceData(view.SpaceDataId);
		await _init();
	}

	private async Task _init()
	{
		var navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		_items = navState.Sorts ?? new();
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
