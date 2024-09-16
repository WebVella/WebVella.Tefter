namespace WebVella.Tefter.Web.Layout;
public partial class SpaceLayout : FluxorLayout
{
	[Inject] protected IState<TfState> TfState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
}