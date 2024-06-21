using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Layout;
public partial class GuestLayout : FluxorLayout
{
    [Inject] private ICryptoService CryptoService { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] protected ITfService TfService { get; set; }
    [Inject] protected NavigationManager Navigator { get; set; }
    [Inject] protected IState<UserState> UserState { get; set; }
    [Inject] protected IDispatcher dispatcher { get; set; }

    private bool _firstRender = true;
    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            Console.WriteLine("FirstRenderAsync");
            Guid? cookieUserId = null;
            var cookieVal = await (new Cookie(JSRuntime)).GetValue(Constants.TEFTER_AUTH_COOKIE_NAME);
            if (String.IsNullOrWhiteSpace(cookieVal) && Guid.TryParse(cookieVal, out Guid outGuid))
                cookieUserId = outGuid;

            if (cookieUserId is null)
            {
                _isLoading = false;
                await InvokeAsync(StateHasChanged);
                return;
            }
            //the the user layout check
            Navigator.NavigateTo(TfConstants.HomePageUrl);
        }
    }

}