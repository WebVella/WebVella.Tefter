using Microsoft.AspNetCore.Localization;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Layout;
public partial class UserLayout : FluxorLayout
{
    [Inject] private ICryptoUtilityService CryptoUtilityService { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IKeyCodeService KeyCodeService { get; set; }
    [Inject] protected ITfService TfService { get; set; }
    [Inject] protected NavigationManager Navigator { get; set; }
    [Inject] protected IState<UserState> UserState { get; set; }
    [Inject] protected IState<SessionState> SessionState { get; set; }
    [Inject] protected IDispatcher dispatcher { get; set; }

    private bool _isLoading = true;
    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            KeyCodeService.UnregisterListener(OnKeyDownAsync);
            UserState.StateChanged -= UserState_StateChanged;
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
            KeyCodeService.RegisterListener(OnKeyDownAsync);
            Navigator.LocationChanged += Navigator_LocationChanged;
            await FirstRenderAsync();
        }
    }

    public async Task FirstRenderAsync()
    {
        Console.WriteLine("FirstRenderAsync");

        Guid? cookieUserId = null;
        var cookieVal = await (new Cookie(JSRuntime)).GetValue(Constants.TEFTER_AUTH_COOKIE_NAME);
        if(!String.IsNullOrWhiteSpace(cookieVal))
            cookieVal = CryptoUtilityService.Decrypt(cookieVal);
        if (!String.IsNullOrWhiteSpace(cookieVal) && Guid.TryParse(cookieVal, out Guid outGuid))
            cookieUserId = outGuid;

        UserState.StateChanged += UserState_StateChanged;
        dispatcher.Dispatch(new GetUserAction(cookieUserId));
    }

    /// <summary>
    /// Sets User State, Sets Authentication cookie and calls for Session State init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UserState_StateChanged(object sender, EventArgs e)
    {
        Console.WriteLine("UserState_StateChanged");

        InvokeAsync(async () =>
        {
            if (UserState.Value.Loading) return;

            if (UserState.Value.User is null)
            {
                _redirectToLogin();
                return;
            }
            //Trigger session init
            var urlData = NavigatorExt.GetUrlData(Navigator);
            SessionState.StateChanged += SessionState_StateChanged;
            dispatcher.Dispatch(new GetSessionAction(
                userId: UserState.Value.User.Id,
                spaceId: urlData.SpaceId,
                spaceDataId: urlData.SpaceDataId,
                spaceViewId: urlData.SpaceViewId));



        });
    }

    /// <summary>
    /// Sets the Session states and shows the contents
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SessionState_StateChanged(object sender, EventArgs e)
    {
        Console.WriteLine("SessionState_StateChanged");
        InvokeAsync(async () =>
        {
            if (SessionState.Value.IsLoading) return;
            _isLoading = false;
            var culture = SessionState.Value.CultureOption is null ? TfConstants.CultureOptions[0].CultureInfo : CultureInfo.GetCultureInfo(SessionState.Value.CultureOption.CultureCode);
            if (culture != CultureInfo.CurrentCulture)
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                await new Cookie(JSRuntime).SetValue(CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(
                            new RequestCulture(
                                culture,
                                culture)));
            }
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
        if (UserState is null || UserState.Value is null
        || UserState.Value.Loading || UserState.Value.User is null) return;

        var urlData = NavigatorExt.GetUrlData(Navigator);

        if (urlData.SpaceId == SessionState.Value.SpaceRouteId
           && urlData.SpaceDataId == SessionState.Value.SpaceDataRouteId
           && urlData.SpaceViewId == SessionState.Value.SpaceViewRouteId) return;


        dispatcher.Dispatch(new GetSessionAction(
                userId: UserState.Value.User.Id,
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
        userState: UserState,
        args: args);

        return Task.CompletedTask;
    }


    private void _redirectToLogin(){
        _isLoading = true;
        StateHasChanged();
        Navigator.NavigateTo(TfConstants.LoginPageUrl);
    }
}