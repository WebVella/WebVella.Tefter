namespace WebVella.Tefter.Web.Pages;
public partial class FastAccessPage : TfBasePage
{
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

}