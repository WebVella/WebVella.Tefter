namespace WebVella.Tefter.UseCases.Space;
public partial class SpaceUseCase
{
	internal async Task InitForState()
	{
		await InitState(null);
	}

	internal Task InitState(string url)
	{
		var urlData = _navigationManager.GetUrlData(url);
		if (urlData.SpaceId is not null)
		{
			var serviceResult = _spaceManager.GetSpace(urlData.SpaceId.Value);
			if (serviceResult.IsFailed)
			{
				ResultUtils.ProcessServiceResult(
					result: Result.Fail(new Error("GetSpace failed").CausedBy(serviceResult.Errors)),
					toastErrorMessage: "Unexpected Error",
					notificationErrorTitle: "Unexpected Error",
					toastService: _toastService,
					messageService: _messageService
				);
				return Task.CompletedTask;
			}


			if (serviceResult.Value is not null)
			{
				_dispatcher.Dispatch(new SetSpaceAction(
					isBusy: false,
						space: new TucSpace(serviceResult.Value),
						routeSpaceId:urlData.SpaceId,
						routeSpaceDataId:urlData.SpaceDataId,
						routeSpaceViewId:urlData.SpaceViewId));
				return Task.CompletedTask;
			}

			_navigationManager.NotFound();
		}
		else
		{
			_dispatcher.Dispatch(new SetSpaceAction(
						isBusy: false,
						space: null,
						routeSpaceId:urlData.SpaceId,
						routeSpaceDataId:urlData.SpaceDataId,
						routeSpaceViewId:urlData.SpaceViewId));
		}
		return Task.CompletedTask;
	}
}