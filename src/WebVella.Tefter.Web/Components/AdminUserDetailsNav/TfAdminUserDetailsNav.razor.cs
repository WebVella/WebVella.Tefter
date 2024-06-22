using WebVella.Tefter.Web.Components.UserManageDialog;

namespace WebVella.Tefter.Web.Components.AdminUserDetailsNav;
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	[Parameter]
	public Guid UserId { get; set;}

	private List<MenuItem> menu = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu(UserId);
		ActionSubscriber.SubscribeToAction<SetCurrentAdminUser>(this, (action) =>
		{
			GenerateMenu(action.User is null ? UserId : action.User.Id);
			StateHasChanged();
		});


	}

	private void GenerateMenu(Guid _userId){
		menu.Clear();
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, _userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.PersonInfo(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, _userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Key(),
			Title = LOC("Access")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, _userId),
			Match = NavLinkMatch.All,
			Icon = new Icons.Regular.Size20.Save(),
			Title = LOC("Saved Views")
		});
	}
}