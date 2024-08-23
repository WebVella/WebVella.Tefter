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
			TucSpace space = GetSpace(urlData.SpaceId.Value);
			if (space is not null)
			{
				TucSpaceData spaceData = null;
				TucSpaceView spaceView = null;
				if (urlData.SpaceDataId is not null)
				{
					var reqItem = GetSpaceData(urlData.SpaceDataId.Value);
					if (reqItem.SpaceId == space.Id)
						spaceData = reqItem;
					else
					{
						ResultUtils.ProcessServiceResult(
							result: Result.Fail(new Error("The requested space data is not part of the current space")),
							toastErrorMessage: "Unexpected Error",
							notificationErrorTitle: "Unexpected Error",
							toastService: _toastService,
							messageService: _messageService
						);
						return Task.CompletedTask;
					}

				}
				if (urlData.SpaceViewId is not null)
				{
					//TODO
				}
				List<TucSpaceData> spaceDataList = GetSpaceDataList(space.Id);
				List<TucSpaceView> spaceViewList = GetSpaceViewList(space.Id);

				_dispatcher.Dispatch(new SetSpaceStateAction(
					isBusy: false,
						space: space,
						spaceData: spaceData,
						spaceDataList: spaceDataList,
						spaceView: spaceView,
						spaceViewList: spaceViewList,
						routeSpaceId: urlData.SpaceId,
						routeSpaceDataId: urlData.SpaceDataId,
						routeSpaceViewId: urlData.SpaceViewId));
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
						spaceDataList: new(),
						spaceViewList: new(),
						routeSpaceId: urlData.SpaceId,
						routeSpaceDataId: urlData.SpaceDataId,
						routeSpaceViewId: urlData.SpaceViewId));
		}
		return Task.CompletedTask;
	}
}