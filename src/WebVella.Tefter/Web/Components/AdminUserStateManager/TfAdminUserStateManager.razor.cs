
namespace WebVella.Tefter.Web.Components.AdminUserStateHandler;
public partial class TfAdminUserStateManager : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			Dispatcher.Dispatch(new EmptyUserAdminAction());
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.OnInitializedAsync(
			initForm: false,
			initMenu: false
		);
		await _initState(null);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private async Task _initState(string url)
	{
		var urlData = Navigator.GetUrlData(url);
		if (urlData.UserId is not null)
		{
			var userResult = await UC.GetUserAsync(urlData.UserId.Value);
			ProcessServiceResponse(userResult);

			if (userResult.IsSuccess)
			{
				if (userResult.Value is not null)
				{
					Dispatcher.Dispatch(new SetUserAdminAction(false, userResult.Value));
					return;
				}
			}
			Navigator.NotFound();
		}
		else
		{
			Dispatcher.Dispatch(new SetUserAdminAction(false, null));
		}
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			await _initState(e.Location);
		});
	}
}