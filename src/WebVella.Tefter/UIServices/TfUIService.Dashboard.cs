namespace WebVella.Tefter.UIServices;

public partial interface ITfUIService
{
	Task<TfAdminDashboardData> GetAdminDashboardData();
	TfHomeDashboardData GetHomeDashboardData(Guid userId);
}
public partial class TfUIService : ITfUIService
{
	public async Task<TfAdminDashboardData> GetAdminDashboardData()
		=> await _tfService.GetAdminDashboardData();
	public TfHomeDashboardData GetHomeDashboardData(Guid userId)
		=> _tfService.GetHomeDashboardData(userId);


}
