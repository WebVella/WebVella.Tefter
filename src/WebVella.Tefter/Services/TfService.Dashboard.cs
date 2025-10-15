namespace WebVella.Tefter.Services;

public partial interface ITfService
{
	public Task<TfAdminDashboardData> GetAdminDashboardData();
}

public partial class TfService : ITfService
{
	public Task<TfAdminDashboardData> GetAdminDashboardData()
	{
		try
		{
			var result = new TfAdminDashboardData();
			result.ProvidersInfo = GetDataProvidersInfo();
			result.SyncInfo = result.ProvidersInfo.Where(x => x.NextSyncOn is not null).OrderBy(x => x.NextSyncOn).Take(5).ToList();

			return Task.FromResult(result);
		}
		catch (Exception ex)
		{
			throw ProcessException(ex);
		}
	}


}
