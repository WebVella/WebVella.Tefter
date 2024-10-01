
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminUserDetailsNav.TfAdminUserDetailsNav","WebVella.Tefter")]
public partial class TfAdminUserDetailsNav : TfBaseComponent
{
	[Inject] private IState<TfRouteState> TfRouteState { get; set; }

	internal List<TucMenuItem> _getMenu()
	{
		var menu = new List<TucMenuItem>();
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, TfRouteState.Value.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.PersonInfo(),
			Title = LOC("Details")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, TfRouteState.Value.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Key(),
			Title = LOC("Access")
		});
		menu.Add(new TucMenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, TfRouteState.Value.UserId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Save(),
			Title = LOC("Saved Views")
		});

		return menu;
	}
}