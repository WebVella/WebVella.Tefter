


namespace WebVella.Tefter.Web.Components.AdminUserDetails;
public partial class TfAdminUserDetails : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }
	[Inject] protected IState<UserAdminState> UserAdminState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.OnInitializedAsync(
			initForm:false,
			initMenu:false
		);

		ActionSubscriber.SubscribeToAction<UserAdminChangedAction>(this, On_GetUserDetailsActionResult);
	}

	private void On_GetUserDetailsActionResult(UserAdminChangedAction action)
	{
		StateHasChanged();
	}

	
}