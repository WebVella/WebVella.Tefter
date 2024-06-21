using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Localization;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Layout;
public partial class UserLayout : FluxorLayout
{
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private ICryptoService CryptoService { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IKeyCodeService KeyCodeService { get; set; }
    [Inject] protected ITfService TfService { get; set; }
    [Inject] protected NavigationManager Navigator { get; set; }
    [Inject] protected IState<SessionState> SessionState { get; set; }
    [Inject] protected IDispatcher dispatcher { get; set; }

    private bool _isLoading = true;
    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            KeyCodeService.UnregisterListener(OnKeyDownAsync);
            SessionState.StateChanged -= SessionState_StateChanged;
            Navigator.LocationChanged -= Navigator_LocationChanged;
        }

        await base.DisposeAsyncCore(disposing);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity is not null && user.Identity.IsAuthenticated)
			{
                var tfUser = ((TfIdentity)authState.User.Identity).User;
		        dispatcher.Dispatch(new SetUserAction(tfUser));
				//Trigger session init
				var urlData = NavigatorExt.GetUrlData(Navigator);
				SessionState.StateChanged += SessionState_StateChanged;
				dispatcher.Dispatch(new GetSessionAction(
					userId: tfUser.Id,
					spaceId: urlData.SpaceId,
					spaceDataId: urlData.SpaceDataId,
					spaceViewId: urlData.SpaceViewId));
			}
			else
			{
				_redirectToLogin();
			}

			KeyCodeService.RegisterListener(OnKeyDownAsync);
            Navigator.LocationChanged += Navigator_LocationChanged;
        }
    }

    /// <summary>
    /// Sets the Session states and shows the contents
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SessionState_StateChanged(object sender, EventArgs e)
    {
        InvokeAsync(async () =>
        {
            if (SessionState.Value.IsLoading) return;

			Console.WriteLine("SessionState_StateChanged");

            var culture = SessionState.Value.CultureOption is null ? TfConstants.CultureOptions[0].CultureInfo : CultureInfo.GetCultureInfo(SessionState.Value.CultureOption.CultureCode);
            if (culture != CultureInfo.CurrentCulture)
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                await new CookieService(JSRuntime).SetAsync(CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(
                            new RequestCulture(
                                culture,
                                culture)),null);
            }
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
        });
    }

    /// <summary>
    /// Monitors Navigation changes in order to get and set the correct values of 
    /// Space data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Navigator_LocationChanged(object sender, EventArgs e)
    {
        Console.WriteLine("Navigator_LocationChanged");
        InvokeAsync(async () =>
        {
            if (_isLoading) return;
            _isLoading = true;
            await InvokeAsync(StateHasChanged);
            _initLocationChange();
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        });
    }

    /// <summary>
    /// Performs the needed changes on location change
    /// </summary>
    private void _initLocationChange()
    {
        var urlData = NavigatorExt.GetUrlData(Navigator);

        if (urlData.SpaceId == SessionState.Value.SpaceRouteId
           && urlData.SpaceDataId == SessionState.Value.SpaceDataRouteId
           && urlData.SpaceViewId == SessionState.Value.SpaceViewRouteId) return;


        dispatcher.Dispatch(new GetSessionAction(
                userId: SessionState.Value.UserId,
                spaceId: urlData.SpaceId,
                spaceDataId: urlData.SpaceDataId,
                spaceViewId: urlData.SpaceViewId));
    }


    /// <summary>
    /// Monitors Key events globally
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task OnKeyDownAsync(FluentKeyCodeEventArgs args)
    {
        Console.WriteLine("keydown");
        DispatchUtils.DispatchKeyDown(
        dispatcher: dispatcher,
        sessionState: SessionState,
        args: args);

        return Task.CompletedTask;
    }


    private void _redirectToLogin(){
        _isLoading = true;
        StateHasChanged();
        Navigator.NavigateTo(TfConstants.LoginPageUrl);
    }
}