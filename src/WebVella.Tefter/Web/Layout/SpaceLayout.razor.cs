namespace WebVella.Tefter.Web.Layout;
public partial class SpaceLayout : LayoutComponentBase
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
}