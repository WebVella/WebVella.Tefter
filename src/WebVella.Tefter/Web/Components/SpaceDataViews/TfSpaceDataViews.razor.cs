namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceDataViews.TfSpaceDataViews","WebVella.Tefter")]
public partial class TfSpaceDataViews : TfFormBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] private SpaceUseCase UC { get; set; }

	private List<TucSpaceView> _items = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		_generateItems();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
	}
	private void On_StateChanged(SpaceStateChangedAction action)
	{
		_generateItems();
		StateHasChanged();
	}

	private void _generateItems()
	{
		_items = SpaceState.Value.SpaceViewList.Where(x => x.SpaceDataId == SpaceState.Value.SpaceData.Id).ToList();
		foreach (var item in _items)
		{
			item.OnClick = () => _navigateToView(item);
		}
	}

	private void _navigateToView(TucSpaceView view){ 
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl,view.SpaceId,view.Id));
	}
}
