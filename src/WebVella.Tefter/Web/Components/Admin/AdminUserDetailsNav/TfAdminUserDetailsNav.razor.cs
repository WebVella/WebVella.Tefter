
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserDetailsNav.TfAdminUserDetailsNav","WebVella.Tefter")]
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	[Inject] private IState<TfAppState> TfAppState { get; set; }

	internal List<TucMenuItem> _getMenu()
	{
		var menu = new List<TucMenuItem>();
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, TfAppState.Value.Route.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.PersonInfo(),
			Text = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, TfAppState.Value.Route.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Key(),
			Text = LOC("Access")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, TfAppState.Value.Route.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Save(),
			Text = LOC("Saved Views")
		});

		return menu;
	}
}