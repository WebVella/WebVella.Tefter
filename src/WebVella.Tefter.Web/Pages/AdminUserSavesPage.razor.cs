namespace WebVella.Tefter.Web.Pages;
public partial class AdminUserSavesPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Inject] protected IState<SessionState> SessionState { get; set; }

}