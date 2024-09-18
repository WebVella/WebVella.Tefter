namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceDataViews.TfSpaceDataViews","WebVella.Tefter")]
public partial class TfSpaceDataViews : TfFormBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	private List<TucSpaceView> _generateItems()
	{
		var items = TfAppState.Value.SpaceViewList.Where(x => x.SpaceDataId == TfAppState.Value.SpaceData.Id).ToList();
		foreach (var item in items)
		{
			item.OnClick = () => _navigateToView(item);
		}
		return items;
	}

	private void _navigateToView(TucSpaceView view){ 
		Navigator.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl,view.SpaceId,view.Id));
	}
}
