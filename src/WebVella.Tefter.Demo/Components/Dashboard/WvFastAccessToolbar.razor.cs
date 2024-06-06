namespace WebVella.Tefter.Demo.Components;
public partial class WvFastAccessToolbar : WvBaseComponent
{
	private string _search = null;

	private async Task searchChanged(string search)
	{
		_search = search;
		await NavigatorExt.SetParamToUrlQuery(Navigator, WvConstants.WvSearchQuery, search);
	}
}