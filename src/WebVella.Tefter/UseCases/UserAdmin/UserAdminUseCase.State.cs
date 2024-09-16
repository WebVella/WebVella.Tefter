namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal Task InitForStateAsync()
	{
		return Task.CompletedTask;
	}

	internal async Task<TucUser> GetUserFromUrl(string url)
	{
		var urlData = _navigationManager.GetRouteState(url);
		if (urlData.UserId is not null)
		{
			var userResult = await _identityManager.GetUserAsync(urlData.UserId.Value);
			if (userResult.IsFailed)
			{
				ResultUtils.ProcessServiceResult(
					result: Result.Fail(new Error("GetUsersAsync failed").CausedBy(userResult.Errors)),
					toastErrorMessage: "Unexpected Error",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
				return null;
			}

			if (userResult.Value is not null)
			{
				return new TucUser(userResult.Value);
			}
			_navigationManager.NotFound();
		}
		return null;
	}
}
