namespace WebVella.Tefter.UIServices;

public partial interface ITfDashboardUIService
{
	Task<TfAdminDashboardData> GetAdminDashboardData();
	TfHomeDashboardData GetHomeDashboardData(Guid userId);
}
public partial class TfDashboardUIService : ITfDashboardUIService
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

	public async Task<TfAdminDashboardData> GetAdminDashboardData()
		=> await _tfService.GetAdminDashboardData();
	public TfHomeDashboardData GetHomeDashboardData(Guid userId)
		=> _tfService.GetHomeDashboardData(userId);


}
