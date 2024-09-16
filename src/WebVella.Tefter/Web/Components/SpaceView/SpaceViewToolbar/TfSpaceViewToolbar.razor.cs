namespace WebVella.Tefter.Web.Components;
public partial class TfSpaceViewToolbar : TfBaseComponent
{
    [Inject] protected IState<TfState> TfState { get; set; }

    private List<ScreenRegionComponent> _regionComponents = new();
    private long _lastRegionRenderedTimestamp = 0;

}