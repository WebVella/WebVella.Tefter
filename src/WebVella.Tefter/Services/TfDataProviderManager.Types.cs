namespace WebVella.Tefter;

public partial interface ITfDataProviderManager
{
	ITfDataProviderType GetProviderType(Guid id);
	ReadOnlyCollection<ITfDataProviderType> GetProviderTypes();
}

public partial class TfDataProviderManager : ITfDataProviderManager
{
	public ITfDataProviderType GetProviderType(Guid id)
	{
		return _providerTypes.SingleOrDefault(x=>x.Id == id);
	}

	public ReadOnlyCollection<ITfDataProviderType> GetProviderTypes()
	{
		return _providerTypes.AsReadOnly();
	}
}
