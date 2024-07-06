
namespace WebVella.Tefter.Web.Components.AdminUserDetailsNav;
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		ActionSubscriber.SubscribeToAction<UserAdminChangedAction>(this, On_GetUserDetailsActionResult);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private async void On_GetUserDetailsActionResult(UserAdminChangedAction action)
	{
		UC.InitDetailsNavMenu();
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		UC.InitDetailsNavMenu();
		StateHasChanged();
	}
}