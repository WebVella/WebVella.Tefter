using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Layout;
public partial class GuestLayout : FluxorLayout
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] protected NavigationManager Navigator { get; set; }


    private bool _isLoading = true;
    private bool _firstRender = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
            var cookie = await new CookieService(JSRuntime).GetAsync(Constants.TEFTER_AUTH_COOKIE_NAME);

            //Temp fix
            if (cookie is not null && user.Identity is not null && user.Identity.IsAuthenticated)
			{
				Navigator.NavigateTo(TfConstants.HomePageUrl,true);
                return;
			}
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
    }

}