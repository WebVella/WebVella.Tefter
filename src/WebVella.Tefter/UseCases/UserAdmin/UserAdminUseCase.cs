namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	private readonly IIdentityManager _identityManager;
	private readonly NavigationManager _navigationManager;
	public UserAdminUseCase(
		IIdentityManager identityManager, NavigationManager navigationManager)
	{
		_identityManager = identityManager;
		_navigationManager = navigationManager;
	}
	internal async Task OnInitializedAsync(
		bool initForm,
		bool initMenu
	)
	{
		if(initForm) await InitForm();
		if(initMenu) await InitMenuAsync();
	}



}
