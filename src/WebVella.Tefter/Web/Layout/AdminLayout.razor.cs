namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : LayoutComponentBase
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	
}