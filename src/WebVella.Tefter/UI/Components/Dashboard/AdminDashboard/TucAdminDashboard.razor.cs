
namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDashboard : TfBaseComponent
{

	private TfAdminDashboardData? _data = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_data = await TfService.GetAdminDashboardData();
	}

}