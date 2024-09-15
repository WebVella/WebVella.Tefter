namespace WebVella.Tefter.UseCases.UserAdmin;
public partial class UserAdminUseCase
{
	internal async Task InitForStateAsync()
	{
		await InitState(null);
	}

	internal async Task InitState(string url)
	{
		var urlData = _navigationManager.GetUrlData(url);
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
				return;
			}

			if (userResult.Value is not null)
			{
				_dispatcher.Dispatch(new SetUserAdminAction(new TucUser(userResult.Value)));
				return;
			}
			_navigationManager.NotFound();
		}
		else
		{
			_dispatcher.Dispatch(new SetUserAdminAction(null));
		}

	}
}
