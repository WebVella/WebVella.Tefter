﻿
namespace WebVella.Tefter.Web.Components.DashboardStateManager;
public partial class TfDashboardStateManager : TfBaseComponent
{
	[Inject] private DashboardUseCase UC { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			//Dispatcher.Dispatch(new EmptyDataProviderAdminAction());
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		Navigator.LocationChanged += Navigator_LocationChanged;
	}


	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		InvokeAsync(async()=>{ 
			await UC.InitState(e.Location);
		});
	}
}