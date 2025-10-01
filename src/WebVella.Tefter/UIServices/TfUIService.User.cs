
namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	//Events
	event EventHandler<TfUser?> CurrentUserChanged;
	event EventHandler<TfUser> UserUpdated;
	event EventHandler<TfUser> UserCreated;

	//User
	ReadOnlyCollection<TfUser> GetUsers(string? search = null);
	ReadOnlyCollection<TfUser> GetUsersForRole(Guid roleId);
	TfUser GetUser(Guid userId);
	Task<TfUser?> GetCurrentUserAsync();
	Task<TfUser> CreateUserWithFormAsync(TfUserManageForm form);
	Task<TfUser> UpdateUserWithFormAsync(TfUserManageForm form);

	//Access
	bool UserHasAccess(TfUser user, NavigationManager navigator);
	Task<TfUser?> AuthenticateAsync(TfLoginForm form);

	Task LogoutAsync();

	//Roles
	Task<TfUser> AddRoleToUserAsync(Guid roleId, Guid userId);
	Task<TfUser> RemoveRoleFromUserAsync(Guid roleId, Guid userId);

	//Settings

	Task<TfUser> SetStartUpUrl(Guid userId, string url);
	Task<TfUser> SetUserCulture(Guid userId, string cultureCode);
	Task<TfUser> SetPageSize(Guid userId, int? pageSize);
	Task<TfUser> SetViewPresetColumnPersonalization(Guid userId, Guid spaceViewId, Guid? preset, Guid spaceViewColumnId, short? width);
	List<TfSortQuery> CalculateViewPresetSortPersonalization(List<TfSortQuery> currentSorts, Guid spaceViewId, Guid spaceViewColumnId, bool hasShiftKey);
	Task<TfUser> RemoveSpaceViewPersonalizations(Guid userId, Guid spaceViewId, Guid? presetId);


	//Bookmarks
	List<TfBookmark> GetUserBookmarks(Guid userId);
	List<TfBookmark> GetUserSaves(Guid userId);

	void CreateBookmark(TfBookmark bookmark);
	void UpdateBookmark(TfBookmark bookmark);
	void DeleteBookmark(TfBookmark bookmark);
}
public partial class TfUIService : ITfUIService
{

	#region << Events >>
	public event EventHandler<TfUser?> CurrentUserChanged = null!;
	public event EventHandler<TfUser> UserUpdated = null!;
	public event EventHandler<TfUser> UserCreated = null!;
	#endregion

	#region << User >>
	public TfUser GetUser(Guid userId) => _tfService.GetUser(userId);
	public ReadOnlyCollection<TfUser> GetUsers(string? search = null) => _tfService.GetUsers(search);
	public ReadOnlyCollection<TfUser> GetUsersForRole(Guid roleId) => _tfService.GetUsersForRole(roleId);
	public async Task<TfUser?> GetCurrentUserAsync()
	=> await _tfService.GetUserFromCookieAsync(_jsRuntime, _authStateProvider);

	public async Task<TfUser> CreateUserWithFormAsync(TfUserManageForm form)
	{
		var user = new TfUser
		{
			Id = form.Id,
			CreatedOn = DateTime.Now,
			Email = form.Email,
			FirstName = form.FirstName,
			LastName = form.LastName,
			Enabled = form.Enabled,
			Password = form.Password ?? String.Empty,
			Roles = new List<TfRole>().AsReadOnly(),
			Settings = new TfUserSettings
			{
				ThemeColor = form.ThemeColor,
				ThemeMode = form.ThemeMode,
				IsSidebarOpen = form.IsSidebarOpen,
				CultureName = form.Culture?.CultureInfo.Name ?? TfConstants.DefaultCulture.Name,
			}
		};
		user = await _tfService.CreateUserAsync(user);
		UserCreated?.Invoke(this, user);
		return user;
	}

	public async Task<TfUser> UpdateUserWithFormAsync(TfUserManageForm form)
	{
		var user = await _tfService.GetUserAsync(form.Id);
		if (user is null)
			throw new Exception("user not found");
		TfUserBuilder userBuilder = _tfService.CreateUserBuilder(user);
		userBuilder
			.WithEmail(form.Email)
			.WithFirstName(form.FirstName)
			.WithLastName(form.LastName)
			.Enabled(form.Enabled)
			.WithThemeMode(form.ThemeMode)
			.WithThemeColor(form.ThemeColor)
			.WithCultureCode(form.Culture?.CultureInfo.Name ?? TfConstants.DefaultCulture.Name)
			.WithRoles(user.Roles.ToArray());
		if (!String.IsNullOrWhiteSpace(form.Password))
			userBuilder.WithPassword(form.Password);

		user = userBuilder.Build();
		user = await _tfService.SaveUserAsync(user);
		UserUpdated?.Invoke(this, user);
		return user;
	}

	#endregion

	#region << Access >>
	public bool UserHasAccess(TfUser user, NavigationManager navigator)
	{
		return _tfService.UserHasAccess(user, navigator);
	}
	public async Task<TfUser?> AuthenticateAsync(TfLoginForm form)
	{
		var user = await _tfService.AuthenticateAsync(
			jsRuntime: _jsRuntime,
			email: form.Email,
			password: form.Password,
			rememberMe: form.RememberMe);
		CurrentUserChanged?.Invoke(this, user);
		return user;
	}

	public async Task LogoutAsync()
	{
		await _tfService.LogoutAsync(_jsRuntime);
		CurrentUserChanged?.Invoke(this, null);
	}
	#endregion

	#region << Roles >>
	public async Task<TfUser> AddRoleToUserAsync(Guid roleId, Guid userId) => await _tfService.AddUserToRoleAsync(userId: userId, roleId: roleId);
	public async Task<TfUser> RemoveRoleFromUserAsync(Guid roleId, Guid userId) => await _tfService.RemoveUserFromRoleAsync(userId: userId, roleId: roleId);

	#endregion

	#region << Settings >>
	public async Task<TfUser> SetStartUpUrl(Guid userId, string url)
	{
		var user = await _tfService.SetStartUpUrl(userId, url);
		UserUpdated?.Invoke(this, user);
		return await _tfService.SetStartUpUrl(userId, url);
	}

	public async Task<TfUser> SetUserCulture(Guid userId, string cultureCode)
	{
		var user = await _tfService.SetUserCulture(userId, cultureCode);
		UserUpdated?.Invoke(this, user);
		return user;
	}

	public async Task<TfUser> SetPageSize(Guid userId, int? pageSize)
	{
		var user = await _tfService.SetPageSize(userId, pageSize);
		UserUpdated?.Invoke(this, user);
		return user;
	}

	public async Task<TfUser> SetViewPresetColumnPersonalization(Guid userId, Guid spaceViewId, Guid? preset, Guid spaceViewColumnId, short? width)
	{
		var user = await _tfService.SetViewPresetColumnPersonalization(userId, spaceViewId, preset, spaceViewColumnId, width);
		UserUpdated?.Invoke(this, user);
		return user;
	}

	public List<TfSortQuery> CalculateViewPresetSortPersonalization(List<TfSortQuery> currentSorts, Guid spaceViewId, Guid spaceViewColumnId,
		bool hasShiftKey)
	{
		return _tfService.CalculateViewPresetSortPersonalization(currentSorts, spaceViewId, spaceViewColumnId, hasShiftKey);
	}

	public async Task<TfUser> RemoveSpaceViewPersonalizations(Guid userId, Guid spaceViewId, Guid? presetId)
	{
		var user = await _tfService.RemoveSpaceViewPersonalizations(userId, spaceViewId, presetId);
		UserUpdated?.Invoke(this, user);
		return user;
	}
	#endregion

	#region << Bookmarks >>
	public List<TfBookmark> GetUserBookmarks(Guid userId)
		=> _tfService.GetBookmarksListForUser(userId).Where(x => String.IsNullOrWhiteSpace(x.Url)).ToList();

	public List<TfBookmark> GetUserSaves(Guid userId)
		=> _tfService.GetBookmarksListForUser(userId).Where(x => !String.IsNullOrWhiteSpace(x.Url)).ToList();

	public void CreateBookmark(TfBookmark bookmark)
	{
		_tfService.CreateBookmark(bookmark);
		var user = GetUser(bookmark.UserId);
		UserUpdated?.Invoke(this, user);
	}
	public void UpdateBookmark(TfBookmark bookmark)
	{
		_tfService.UpdateBookmark(bookmark);
		var user = GetUser(bookmark.UserId);
		UserUpdated?.Invoke(this, user);
	}
	public void DeleteBookmark(TfBookmark bookmark)
	{
		_tfService.DeleteBookmark(bookmark.Id);
		var user = GetUser(bookmark.UserId);
		UserUpdated?.Invoke(this, user);
	}
	#endregion
}
