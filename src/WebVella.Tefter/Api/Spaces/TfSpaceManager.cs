using WebVella.Tefter.Api;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
}

public partial class TfSpaceManager : ITfSpaceManager
{
	private readonly IDboManager _dboManager;
	private readonly IDatabaseService _dbService;
	private readonly ITfDataProviderManager _providerManager;
	private readonly ITfMetaProvider _metaProvider;
	private readonly IServiceProvider _serviceProvider;

	public TfSpaceManager(IServiceProvider serviceProvider)
	{
		_dboManager = serviceProvider.GetService<IDboManager>();
		_dbService = serviceProvider.GetService<IDatabaseService>();
		_providerManager = serviceProvider.GetService<ITfDataProviderManager>();
		_metaProvider = serviceProvider.GetService<ITfMetaProvider>();
		_serviceProvider = serviceProvider;
	}
}
