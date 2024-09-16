namespace WebVella.Tefter.Web.Pages;
public partial class FastAccessPage : TfBasePage
{

	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }

}