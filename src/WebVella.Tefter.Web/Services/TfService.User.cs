using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	ValueTask<User> GetUserByIdAsync(Guid userId);
	ValueTask<User> GetUserFromBrowserStorage();
	Task LogoutUser();
}

public partial class TfService : ITfService
{
    public async ValueTask<User> GetUserFromBrowserStorage()
	{
		try
		{
			var result = await browserStorage.GetAsync<Guid?>(TfConstants.UserLocalKey);
			if (!result.Success
			 || result.Value is null
			 || result.Value.Value == Guid.Empty) return null;

			var userId = result.Value.Value;
			return await GetUserByIdAsync(userId);

		}
		catch (Exception)
		{
			//Errors can be because of wrong decrytion or old object
			//Delete and return null
			await browserStorage.DeleteAsync(TfConstants.UserLocalKey);
			return null;
		}
	}

	public async ValueTask<User> GetUserByIdAsync(Guid userId)
	{
		Result<User> userResult = await identityManager.GetUserAsync(userId);
		if (userResult.IsFailed) throw new Exception("getting user failed");
		if (userResult.Value is null) throw new Exception("user not found");
		return userResult.Value;
	}

	public async Task LogoutUser()
	{
		//await browserStorage.DeleteAsync(TfConstants.UserLocalKey);
		await RemoveUnprotectedLocalStorage(TfConstants.UIThemeLocalKey);
	}
}
