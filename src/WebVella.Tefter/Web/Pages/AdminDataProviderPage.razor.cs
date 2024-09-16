namespace WebVella.Tefter.Web.Pages;
public partial class AdminDataProviderPage : TfBasePage
{
	[Parameter] public Guid ProviderId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfUserState> TfUserState { get; set; }


}