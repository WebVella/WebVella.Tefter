namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	Result<ITfDataProviderType> GetProviderType(Guid id);
	Result<ReadOnlyCollection<ITfDataProviderType>> GetProviderTypes();
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public Result<ITfDataProviderType> GetProviderType(Guid id)
	{
		var provider = _providerTypes.SingleOrDefault(x=>x.Id == id);
		return Result.Ok(provider);
	}

	public Result<ReadOnlyCollection<ITfDataProviderType>> GetProviderTypes()
	{
		return Result.Ok(_providerTypes.AsReadOnly());
	}
}
