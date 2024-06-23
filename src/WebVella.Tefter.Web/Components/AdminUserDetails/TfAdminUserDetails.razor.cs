


namespace WebVella.Tefter.Web.Components.AdminUserDetails;
public partial class TfAdminUserDetails : TfBaseComponent
{
	[Inject] protected IState<UserDetailsState> UserDetailsState { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Dispatcher.Dispatch(new EmptyUserDetailsAction());
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			_getUser();
			Navigator.LocationChanged += Navigator_LocationChanged;
		}
	}

	private void On_GetUserDetailsActionResult(GetUserDetailsActionResult action)
	{
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		_getUser();
	}

	private void _getUser()
	{
		var urlData = Navigator.GetUrlData();
		if (urlData.UserId is not null)
		{
			ActionSubscriber.SubscribeToAction<GetUserDetailsActionResult>(this, On_GetUserDetailsActionResult);
			Dispatcher.Dispatch(new GetUserDetailsAction(urlData.UserId.Value));
		}
	}
}