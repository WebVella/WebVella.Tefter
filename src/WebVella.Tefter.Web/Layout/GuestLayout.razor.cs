using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Web.Layout;
public partial class GuestLayout : FluxorLayout
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
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
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity is not null && user.Identity.IsAuthenticated)
			{
				Navigator.NavigateTo(TfConstants.HomePageUrl);
                return;
			}
			_isLoading = false;
			await InvokeAsync(StateHasChanged);

		}
    }

}