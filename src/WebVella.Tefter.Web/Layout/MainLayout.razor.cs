using WebVella.Tefter.Web.Store.SessionState;
using WebVella.Tefter.Web.Store.UserState;

namespace WebVella.Tefter.Web.Layout;
public partial class MainLayout : FluxorLayout
{
	[Inject] protected IState<SessionState> SessionState { get; set; }
}