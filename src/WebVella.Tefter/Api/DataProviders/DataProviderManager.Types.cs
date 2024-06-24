namespace WebVella.Tefter;

public partial interface IDataProviderManager
{
	ReadOnlyCollection<ITfDataProviderType> GetProviderTypes();
}

internal partial class DataProviderManager : IDataProviderManager
{
	public ReadOnlyCollection<ITfDataProviderType> GetProviderTypes()
	{
		return _providerTypes.AsReadOnly();
	}
}
