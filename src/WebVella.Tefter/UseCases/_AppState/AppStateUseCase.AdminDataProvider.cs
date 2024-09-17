namespace WebVella.Tefter.UseCases.AppStart;
internal partial class AppStateUseCase
{
	internal async Task<TfAppState> InitAdminDataProvider(TucUser currentUser, TfRouteState routeState, TfAppState result)
	{
		if (
			!(routeState.FirstNode == RouteDataFirstNode.Admin
			&& routeState.SecondNode == RouteDataSecondNode.DataProviders)
			)
		{
			result = result with { AdminDataProviders = new(), AdminDataProvidersPage = 1, AdminManagedDataProvider = null };
			return result;
		};


		//AdminDataProviders, AdminDataProvidersPage
		if (result.AdminDataProviders.Count == 0)
			result = result with { AdminDataProviders = await GetDataProvidersAsync(null, 1, TfConstants.PageSize), AdminDataProvidersPage = 2 };

		//AdminManagedUser, UserRoles
		if (routeState.DataProviderId.HasValue)
		{
			var adminProvider = await GetProvider(routeState.DataProviderId.Value);
			result = result with { AdminManagedDataProvider = adminProvider };
			if (adminProvider is not null)
			{
				if (!result.AdminDataProviders.Any(x => x.Id == adminProvider.Id))
				{
					var adminProviders = result.AdminDataProviders.ToList();
					adminProviders.Add(adminProvider);
					result = result with { AdminDataProviders = adminProviders };
				}

				//check for the other tabs
				if (routeState.ThirdNode == RouteDataThirdNode.Schema)
				{
				}
				else if (routeState.ThirdNode == RouteDataThirdNode.SharedKeys)
				{
				}
			}
		}

		return result;
	}

	internal Task<List<TucDataProvider>> GetDataProvidersAsync(string search = null, int? page = null, int? pageSize = null)
	{
		var srvResult = _dataProviderManager.GetProviders();
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProviders failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new List<TucDataProvider>());
		}

		if (srvResult.Value is null) return Task.FromResult(new List<TucDataProvider>());

		var records = new List<TfDataProvider>();
		if (!String.IsNullOrWhiteSpace(search))
		{
			var searchProcessed = search.Trim().ToLowerInvariant();
			foreach (var item in srvResult.Value)
			{
				bool hasMatch = false;
				if (item.Name.ToLowerInvariant().Contains(searchProcessed)) hasMatch = true;
				if (hasMatch) records.Add(item);
			}
		}
		else records = srvResult.Value.ToList();

		if (page is null || pageSize is null) return Task.FromResult(records.Select(x => new TucDataProvider(x)).ToList());

		return Task.FromResult(records.Skip(RenderUtils.CalcSkip(page.Value, pageSize.Value)).Take(pageSize.Value).Select(x => new TucDataProvider(x)).ToList());
	}
	internal Task<TucDataProvider> GetProvider(Guid providerId)
	{
		var srvResult = _dataProviderManager.GetProvider(providerId);
		if (srvResult.IsFailed)
		{
			ResultUtils.ProcessServiceResult(
				result: Result.Fail(new Error("GetProvider failed").CausedBy(srvResult.Errors)),
				toastErrorMessage: "Unexpected Error",
				notificationErrorTitle: "Unexpected Error",
				toastService: _toastService,
				messageService: _messageService
			);
			return Task.FromResult(new TucDataProvider());
		}
		if (srvResult.Value is null) return Task.FromResult((TucDataProvider)null);


		return Task.FromResult(new TucDataProvider(srvResult.Value));
	}
	internal Task<Result> DeleteDataProvider(Guid providerId)
	{
		var srvResult = _dataProviderManager.DeleteDataProvider(providerId);
		if (srvResult.IsFailed)
		{
			return Task.FromResult(Result.Fail(new Error("DeleteDataProvider failed").CausedBy(srvResult.Errors)));
		}

		return Task.FromResult(Result.Ok());
	}
}
