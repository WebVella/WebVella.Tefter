namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceViewSortsDialog : TfFormBaseComponent, IDialogContentComponent<Guid>
{
	[Parameter] public Guid Content { get; set; } = Guid.Empty;
	[CascadingParameter] public FluentDialog Dialog { get; set; } = null!;

	private TfDataset _spaceData = null;
	private List<TfSpaceViewColumn> _viewColumns = new();
	private List<TfSortQuery> _items = new();

	private string _activeTab = "current";
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		if (Content == Guid.Empty)
			throw new Exception("SpaceViewId not found");
		var view = TfService.GetSpaceView(Content);
		if (view is null)
			throw new Exception("SpaceView not found");

		_viewColumns = TfService.GetSpaceViewColumnsList(Content);
		_spaceData = TfService.GetDataset(view.DatasetId);
		await _init();
	}

	private async Task _init()
	{
		var navState = TfState.NavigationState;
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
