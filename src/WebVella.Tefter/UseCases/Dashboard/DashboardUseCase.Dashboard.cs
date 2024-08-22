namespace WebVella.Tefter.UseCases.Dashboard;
public partial class DashboardUseCase
{

	internal TucDashboard Dashboard { get; set; } = null;
	internal Task InitDashboard()
	{
		Dashboard = null;
		return Task.CompletedTask;
	}


}