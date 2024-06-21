using WebVella.Tefter.Web.Store.SessionState;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
	private bool _isLoading = true;


	private void _onLoadingChange(bool value)
	{
		_isLoading = value;
		StateHasChanged();
	}
}