namespace WebVella.Tefter.Web.Layout;
public partial class SpaceLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
}