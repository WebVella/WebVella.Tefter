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
				TucSpaceView spaceView = null;
				TucSpaceData spaceData = null;
				if (urlData.SpaceViewId is not null)
				{
					var reqItem = GetSpaceView(urlData.SpaceViewId.Value);
					if (reqItem is null)
					{
						ResultUtils.ProcessServiceResult(
							result: Result.Fail(new Error("Space view not found")),
							toastErrorMessage: "Unexpected Error",
							notificationErrorTitle: "Unexpected Error",
							toastService: _toastService,
							messageService: _messageService
						);
						return Task.CompletedTask;
					}
					if (reqItem.SpaceId == space.Id)
						spaceView = reqItem;
					else
					{
						ResultUtils.ProcessServiceResult(
							result: Result.Fail(new Error("The requested space view is not part of the current space")),
							toastErrorMessage: "Unexpected Error",
							notificationErrorTitle: "Unexpected Error",
							toastService: _toastService,
							messageService: _messageService
						);
						return Task.CompletedTask;
					}
				}
				List<TucSpaceView> spaceViewList = GetSpaceViewList(space.Id);
				if (urlData.SpaceDataId is not null)
				{
					var reqItem = GetSpaceData(urlData.SpaceDataId.Value);
					if (reqItem is null)
					{
						ResultUtils.ProcessServiceResult(
							result: Result.Fail(new Error("Space data not found")),
							toastErrorMessage: "Unexpected Error",
							notificationErrorTitle: "Unexpected Error",
							toastService: _toastService,
							messageService: _messageService
						);
						return Task.CompletedTask;
					}
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
				List<TucSpaceData> spaceDataList = GetSpaceDataList(space.Id);

				#region << Redirects >>
				{
					//redirect to the first view
					if (
						(String.IsNullOrWhiteSpace(urlData.SpaceSection) || urlData.SpaceSection == "view")
						&& urlData.SpaceViewId is null
						&& spaceViewList.Count > 0
						)
					{
						_navigationManager.NavigateTo(String.Format(TfConstants.SpaceViewPageUrl, space.Id, spaceViewList[0].Id));
						return Task.CompletedTask;
					}
					//redirect to the first data
					if (
						urlData.SpaceSection == "data"
						&& urlData.SpaceDataId is null
						&& spaceDataList.Count > 0
						)
					{
						_navigationManager.NavigateTo(String.Format(TfConstants.SpaceDataPageUrl, space.Id, spaceDataList[0].Id));
						return Task.CompletedTask;
					}
				}
				#endregion


				_dispatcher.Dispatch(new SetSpaceStateAction(
					isBusy: false,
						space: space,
						spaceView: spaceView,
						spaceViewList: spaceViewList,
						spaceData: spaceData,
						spaceDataList: spaceDataList,
						routeSpaceId: urlData.SpaceId,
						routeSpaceViewId: urlData.SpaceViewId,
						routeSpaceDataId: urlData.SpaceDataId));
				return Task.CompletedTask;
			}

			_navigationManager.NotFound();
		}
		else
		{
			_dispatcher.Dispatch(new SetSpaceStateAction(
						isBusy: false,
						space: null,
						spaceView: null,
						spaceViewList: new(),
						spaceData: null,
						spaceDataList: new(),
						routeSpaceId: urlData.SpaceId,
						routeSpaceViewId: urlData.SpaceViewId,
						routeSpaceDataId: urlData.SpaceDataId));
		}
		return Task.CompletedTask;
	}
}