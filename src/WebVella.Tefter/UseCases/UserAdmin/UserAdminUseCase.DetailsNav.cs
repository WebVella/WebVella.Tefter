namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal List<MenuItem> DetailsNavMenu { get; set; } = new List<MenuItem>();

	internal Task InitForDetailsNavAsync()
	{
		InitDetailsNavMenu();
		return Task.CompletedTask;
	}

	internal void InitDetailsNavMenu()
	{
		DetailsNavMenu.Clear();
		var userId = _navigationManager.GetUrlData().UserId ?? Guid.Empty;
		DetailsNavMenu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserDetailsPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.PersonInfo(),
			Title = _loc["Details"]
		});
		DetailsNavMenu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserAccessPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Key(),
			Title = _loc["Access"]
		});
		DetailsNavMenu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.AdminUserSavesViewsPageUrl, userId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Save(),
			Title = _loc["Saved Views"]
		});
	}
}
