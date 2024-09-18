namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

}