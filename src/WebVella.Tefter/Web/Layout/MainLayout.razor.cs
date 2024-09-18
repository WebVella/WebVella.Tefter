namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : LayoutComponentBase
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

}