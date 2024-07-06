namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	internal async Task InitForState()
	{
		await InitState(null);
	}

	internal Task InitState(string url)
	{
		var urlData = _navigationManager.GetUrlData(url);
		if (urlData.DataProviderId is not null)
		{
			var serviceResult = _dataProviderManager.GetProvider(urlData.DataProviderId.Value);
			if (serviceResult.IsFailed)
			{
				ResultUtils.ProcessServiceResult(
					result: Result.Fail(new Error("GetProvider failed").CausedBy(serviceResult.Errors)),
					toastErrorMessage: "Unexpected Error",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
				return Task.CompletedTask;
			}


			if (serviceResult.Value is not null)
			{
				_dispatcher.Dispatch(new SetDataProviderAdminAction(
					isBusy: false,
					provider: new TucDataProvider(serviceResult.Value)));
				return Task.CompletedTask;
			}

			_navigationManager.NotFound();
		}
		else
		{
			_dispatcher.Dispatch(new SetDataProviderAdminAction(
						isBusy: false,
						provider: null));
		}
		return Task.CompletedTask;
	}
}
