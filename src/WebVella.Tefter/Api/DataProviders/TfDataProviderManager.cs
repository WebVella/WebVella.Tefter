namespace WebVella.Tefter;

public partial interface ITfDataProviderManager { }

public partial class TfDataProviderManager : ITfDataProviderManager
{
	private readonly IDboManager _dboManager;

	public TfDataProviderManager(IServiceProvider serviceProvider)
	{
		_dboManager = serviceProvider.GetService<IDboManager>();
	}
}
