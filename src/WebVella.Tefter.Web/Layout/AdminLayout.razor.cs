namespace WebVella.Tefter.Web.Layout;
public partial class AdminLayout : FluxorLayout
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private bool _isLoading = true;


	private void _onLoadingChange(bool value)
	{
		_isLoading = value;
		StateHasChanged();
	}
}