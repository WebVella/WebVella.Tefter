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
				TucSpace space = new TucSpace(serviceResult.Value);
				TucSpaceData spaceData = null;
				TucSpaceView spaceView = null;
				if(urlData.SpaceDataId is not null) {
					var reqItem = _spaceManager.GetSpaceData(urlData.SpaceDataId.Value);
					if(reqItem.IsSuccess && reqItem.Value.SpaceId == space.Id) 
						spaceData = new TucSpaceData(reqItem.Value);
					
				}
				if(urlData.SpaceViewId is not null) {
					//TODO
				}


				_dispatcher.Dispatch(new SetSpaceStateAction(
					isBusy: false,
						space: space,
						spaceData: spaceData,
						spaceView: spaceView,
						routeSpaceId:urlData.SpaceId,
						routeSpaceDataId:urlData.SpaceDataId,
						routeSpaceViewId:urlData.SpaceViewId));
				return Task.CompletedTask;
			}

			_navigationManager.NotFound();
		}
		else
		{
			_dispatcher.Dispatch(new SetSpaceStateAction(
						isBusy: false,
						space: null,
						spaceData: null,
						spaceView: null,
						routeSpaceId:urlData.SpaceId,
						routeSpaceDataId:urlData.SpaceDataId,
						routeSpaceViewId:urlData.SpaceViewId));
		}
		return Task.CompletedTask;
	}
}