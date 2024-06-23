
namespace WebVella.Tefter.Web.Components.AdminUserDetailsNav;
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	private List<MenuItem> menu = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}


	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			GenerateMenu();
			ActionSubscriber.SubscribeToAction<GetUserDetailsActionResult>(this, On_GetUserDetailsActionResult);
		}
	}
	private void GenerateMenu()
	{
		menu.Clear();
		var userId = Navigator.GetUrlData().UserId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.PersonInfo(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Key(),
			Title = LOC("Access")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Save(),
			Title = LOC("Saved Views")
		});
	}

	private void On_GetUserDetailsActionResult(GetUserDetailsActionResult action)
	{
		GenerateMenu();
		StateHasChanged();
	}
}