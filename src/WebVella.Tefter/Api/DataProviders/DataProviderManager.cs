namespace WebVella.Tefter;

public partial interface IDataProviderManager { }

internal partial class DataProviderManager : IDataProviderManager
{
	private readonly IDboManager _dboManager;

	public DataProviderManager(IDboManager dboManager)
	{
		_dboManager = dboManager;
	}
}
