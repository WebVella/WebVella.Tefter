namespace WebVella.Tefter.Web.Layout;
public partial class GuestLayout : LayoutComponentBase
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

	[Inject] private IJSRuntime JSRuntime { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }


	private bool _isLoading = true;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			var cookie = await new CookieService(JSRuntime).GetAsync(TfConstants.TEFTER_AUTH_COOKIE_NAME);

			//Temp fix
			if (!(cookie is null || user.Identity is null || !user.Identity.IsAuthenticated ||
				(user.Identity as TfIdentity) is null ||
				(user.Identity as TfIdentity).User is null))
			{
				Navigator.NavigateTo(TfConstants.HomePageUrl, true);
				return;
			}
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}

}