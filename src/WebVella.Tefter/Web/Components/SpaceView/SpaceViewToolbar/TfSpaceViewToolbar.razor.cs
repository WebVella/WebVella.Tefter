namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private List<ScreenRegionComponent> _regionComponents = new();
	private long _lastRegionRenderedTimestamp = 0;

	private async Task _searchChanged(string value)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = value;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}

}