namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceViewPages.TfSpaceViewPages", "WebVella.Tefter")]
public partial class TfSpaceViewPages : TfBaseComponent
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

	private List<TucSpaceNode> _generateItems()
		=> TfAppState.Value.SpaceNodes.Where(x => x.ComponentOptionsJson.Contains(TfAppState.Value.SpaceView.Id.ToString())).ToList();

}
