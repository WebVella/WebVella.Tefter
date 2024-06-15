namespace WebVella.Tefter.Web.Pages;
public partial class SpacePage : TfBasePage
{
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }




}