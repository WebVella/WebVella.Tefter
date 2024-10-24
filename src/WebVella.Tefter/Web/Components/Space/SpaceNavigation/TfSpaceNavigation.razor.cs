using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceView.SpaceNavigation.TfSpaceNavigation", "WebVella.Tefter")]
public partial class TfSpaceNavigation : TfBaseComponent
{
	[Inject] protected IState<TfUserState> TfUserState { get; set; }
	[Inject] protected IState<TfRouteState> TfRouteState { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }

	[Inject] private AppStateUseCase UC { get; set; }

}

