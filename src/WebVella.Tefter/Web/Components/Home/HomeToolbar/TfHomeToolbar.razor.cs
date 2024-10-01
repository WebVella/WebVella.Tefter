namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Home.HomeToolbar.TfHomeToolbar", "WebVella.Tefter")]
public partial class TfHomeToolbar : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Parameter] public EventCallback<string> OnSearch { get; set; }
	[Parameter] public EventCallback<string> OnFilterToggle { get; set; }

	private List<TucScreenRegionComponentMeta> _regionComponents = new();
	private long _lastRegionRenderedTimestamp = 0;

	private async Task _searchChanged(string value) => await OnSearch.InvokeAsync(value);

	private async Task _toggleFilter(string propName) => await OnFilterToggle.InvokeAsync(propName);

}