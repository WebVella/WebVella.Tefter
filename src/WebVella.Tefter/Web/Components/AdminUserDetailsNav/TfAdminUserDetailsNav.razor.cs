
namespace WebVella.Tefter.Web.Components.AdminUserDetailsNav;
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	private List<MenuItem> menu = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu();
		ActionSubscriber.SubscribeToAction<UserAdminChangedAction>(this, On_GetUserDetailsActionResult);
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void GenerateMenu()
	{
		menu.Clear();
		var userId = Navigator.GetUrlData().UserId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.PersonInfo(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Key(),
			Title = LOC("Access")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Save(),
			Title = LOC("Saved Views")
		});
	}

	private void On_GetUserDetailsActionResult(UserAdminChangedAction action)
	{
		GenerateMenu();
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		GenerateMenu();
		StateHasChanged();
	}
}