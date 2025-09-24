
namespace WebVella.Tefter.UI.Components;
public partial class TucAdminDashboard : TfBaseComponent
{
	[Inject] protected ITfDashboardUIService TfGeneralUIService { get; set; } = default!;

	private TfAdminDashboardData? _data = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_data = await TfGeneralUIService.GetAdminDashboardData();
	}
}