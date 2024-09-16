namespace WebVella.Tefter.Web.Components;
public partial class TfStateInit : TfBaseComponent
{
	[Inject] private UserStateUseCase UC { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }

	//protected override async ValueTask DisposeAsyncCore(bool disposing)
	//{
	//	if (disposing)
	//	{
	//		Navigator.LocationChanged -= Navigator_LocationChanged;
	//	}

	//	await base.DisposeAsyncCore(disposing);
	//}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			var state = await UC.InitState();


			//For the logout fix - Not needed anymore
			//Navigator.LocationChanged += Navigator_LocationChanged;
			UC.IsBusy = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	/// <summary>
	/// Fixing a problem when loging out from one tab leaves the user logged on the others
	/// Space data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	//private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	//{
	//	InvokeAsync(async () =>
	//	{
	//		//await UC.OnLocationChange();
	//	});
	//}

}
