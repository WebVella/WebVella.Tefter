namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal Task<TfAppState> InitSpaceViewAsync(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (routeState.SpaceId is null)
		{
			result = result with { SpaceViewList = new(), SpaceView = null };
			return Task.FromResult(result);
		}

		//SpaceViewList
		if (result.SpaceViewList.Count == 0
			|| !result.SpaceViewList.Any(x => x.SpaceId == routeState.SpaceId)
			)
			result = result with { SpaceViewList = GetSpaceViewList(routeState.SpaceId.Value) };

		//Space View
		if (routeState.SpaceViewId is not null)
		{
			result = result with { SpaceView = GetSpaceView(routeState.SpaceViewId.Value) };
		}
		else
		{
			result = result with { SpaceView = null };
		}

		return Task.FromResult(result);
	}

	internal TucSpaceView GetSpaceView(Guid viewId)
	{
		var serviceResult = _spaceManager.GetSpaceView(viewId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceView failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return null;

		return new TucSpaceView(serviceResult.Value);
	}

	internal List<TucSpaceView> GetSpaceViewList(Guid spaceId)
	{
		var serviceResult = _spaceManager.GetSpaceViewsList(spaceId);
		if (serviceResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetSpaceViewsList failed").CausedBy(serviceResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return null;
		}
		if (serviceResult.Value is null) return new();

		return serviceResult.Value.Select(x => new TucSpaceView(x)).ToList();
	}
}
