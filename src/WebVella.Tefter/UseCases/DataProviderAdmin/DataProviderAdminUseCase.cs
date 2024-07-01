namespace WebVella.Tefter.UseCases.DataProviderAdmin;
public partial class DataProviderAdminUseCase
{
	private readonly ITfDataProviderManager _dataProviderManager;
	public DataProviderAdminUseCase(ITfDataProviderManager dataProviderManager)
	{
		_dataProviderManager = dataProviderManager;
	}

	public Result<TucDataProvider> GetProvider(Guid providerId)
	{
		var result = _dataProviderManager.GetProvider(providerId);
		if (result.IsFailed)
			return Result.Fail(new Error("GetProvider failed").CausedBy(result.Errors));
		if (result.Value is null)
			return Result.Fail(new Error("GetProvider - no provider found"));

		return Result.Ok(new TucDataProvider(result.Value));
	}

	public Result<List<TucDatabaseColumnTypeInfo>> GetDbTypesForProviderSourceDataTable(Guid providerId, string dataType)
	{
		var allProviders = _dataProviderManager.GetProviderTypes();
		if(allProviders.IsFailed) return Result.Fail(new Error("GetProviderTypes failed"));
		if(allProviders.Value is null) return Result.Fail(new Error("provider not found"));
		var provider = allProviders.Value.FirstOrDefault(x=> x.Id == providerId);
		if(provider is null) return Result.Fail(new Error("provider not found"));
		var providerDbType = provider.GetDatabaseColumnTypesForSourceDataType(dataType);
		return Result.Ok(providerDbType.Select(x=> new TucDatabaseColumnTypeInfo(x)).ToList());

	}
}
