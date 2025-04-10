namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataData.TfSpaceDataData", "WebVella.Tefter")]
public partial class TfSpaceDataData : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private async Task _goFirstPage()
	{
		if (TfAppState.Value.SpaceDataPage == 1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName,1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = TfAppState.Value.SpaceDataPage - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.SpaceDataPage == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		if (TfAppState.Value.SpaceViewData is null
		|| TfAppState.Value.SpaceViewData.Rows.Count == 0)
			return;

		var page = TfAppState.Value.SpaceDataPage + 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.SpaceDataPage == page) return;

		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (TfAppState.Value.SpaceDataPage == -1) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.SpaceDataPage == page) return;
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


	private async Task _onSearch(string value)
	{
		var queryDict = new Dictionary<string, object>{
			{ TfConstants.SearchQueryName, value}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}
