namespace WebVella.Tefter.UIServices;

public partial interface ITfGeneralUIService
{
	Task<TfAdminDashboardData> GetAdminDashboardData();
}
public partial class TfDashboardUIService : ITfGeneralUIService
{
	private readonly ITfService _tfService;
	private readonly ITfMetaService _metaService;
	private readonly IStringLocalizer<TfDashboardUIService> LOC;

	public TfDashboardUIService(IServiceProvider serviceProvider)
	{
		_tfService = serviceProvider.GetService<ITfService>() ?? default!;
		_metaService = serviceProvider.GetService<ITfMetaService>() ?? default!;
		LOC = serviceProvider.GetService<IStringLocalizer<TfDashboardUIService>>() ?? default!;
	}

	public Task<TfAdminDashboardData> GetAdminDashboardData(){ 
		var result = new TfAdminDashboardData();
		result.ProvidersInfo = _tfService.GetDataProvidersInfo();
		result.SyncInfo = result.ProvidersInfo.Where(x=> x.NextSyncOn is not null).OrderBy(x => x.NextSyncOn).Take(5).ToList();

		return Task.FromResult(result);
	}

}
