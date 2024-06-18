using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Services;
public partial interface ITfService
{
	ValueTask<List<User>> GetUsersAsync();
	ValueTask<User> GetUserFromBrowserStorage();
	ValueTask<User> LoginUserByEmailAndPassword(string email, string password);
	Task LogoutUser();
}

public partial class TfService : ITfService
{
	public async ValueTask<List<User>> GetUsersAsync()
	{
		return await dataBroker.GetUsersAsync();
	}

	public async ValueTask<User> GetUserFromBrowserStorage()
	{
		try
		{
			var result = await browserStorage.GetAsync<Guid?>(TfConstants.UserLocalKey);
			if (!result.Success
			 || result.Value is null
			 || result.Value.Value == Guid.Empty) return null;

			var userId = result.Value.Value;
			return await GetUserById(userId);
		}
		catch (Exception)
		{
			//Errors can be because of wrong decrytion or old object
			//Delete and return null
			await browserStorage.DeleteAsync(TfConstants.UserLocalKey);
			return null;
		}
	}

	public async ValueTask<User> LoginUserByEmailAndPassword(string email, string password)
	{
		var user = await dataBroker.GetUserByEmailAndPasswordAsync(email, password);
		await browserStorage.SetAsync(TfConstants.UserLocalKey, user.Id);
		await browserStorage.SetAsync(TfConstants.UIThemeLocalKey, user.Id);
		return user;
	}

	private async ValueTask<User> GetUserById(Guid userId)
	{
		return await dataBroker.GetUserByIdAsync(userId);
	}

	public async Task LogoutUser()
	{
		await browserStorage.DeleteAsync(TfConstants.UserLocalKey);
		await browserStorage.DeleteAsync(TfConstants.UIThemeLocalKey);
	}
}
