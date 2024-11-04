namespace WebVella.Tefter.Web.Pages;
public partial class AdminUserPage : TfBasePage
{
	[Parameter] public Guid UserId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

}