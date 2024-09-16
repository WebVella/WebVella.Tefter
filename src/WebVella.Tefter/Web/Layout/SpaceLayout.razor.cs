namespace WebVella.Tefter.Web.Layout;
public partial class SpaceLayout : LayoutComponentBase
{
	[Inject] protected IState<TfState> TfState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
}