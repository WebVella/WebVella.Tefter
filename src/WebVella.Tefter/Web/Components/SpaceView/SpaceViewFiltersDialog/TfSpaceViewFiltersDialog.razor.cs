namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewFiltersDialog.TfSpaceViewFiltersDialog", "WebVella.Tefter")]
public partial class TfSpaceViewFiltersDialog : TfFormBaseComponent, IDialogContentComponent<bool>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Parameter] public bool Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }
	private TucDataProvider _dataProvider = null;
	private TucSpaceData _spaceData = null;
	private List<TucFilterBase> _items = new List<TucFilterBase>();
	private string _activeTab = "current";
	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAppState.Value.SpaceViewFilters is not null)
			_items = JsonSerializer.Deserialize<List<TucFilterBase>>(JsonSerializer.Serialize(TfAppState.Value.SpaceViewFilters));
		_spaceData = TfAppState.Value.SpaceDataList.FirstOrDefault(x => x.Id == TfAppState.Value.SpaceView.SpaceDataId);

		if (_spaceData is not null)
		{
			_dataProvider = TfAppState.Value.AllDataProviders.FirstOrDefault(x => x.Id == _spaceData.DataProviderId);
		}
	}


	private async Task _submit()
	{

	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

}
