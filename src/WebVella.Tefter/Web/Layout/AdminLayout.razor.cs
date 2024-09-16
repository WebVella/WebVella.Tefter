namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : FluxorLayout
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	
}