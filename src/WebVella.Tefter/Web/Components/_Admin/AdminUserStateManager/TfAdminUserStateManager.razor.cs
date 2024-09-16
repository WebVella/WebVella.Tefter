
namespace WebVella.Tefter.Web.Components;
public partial class TfAdminUserStateManager : TfBaseComponent
{
	[Inject] private UserAdminUseCase UC { get; set; }

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			Dispatcher.Dispatch(new SetUserAdminAction(component:this,userDetails:null));
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await UC.Init(this.GetType());
		Dispatcher.Dispatch(new SetUserAdminAction(component:this,userDetails:await UC.GetUserFromUrl(Navigator.Uri)));
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		base.InvokeAsync(async () =>
		{
			Dispatcher.Dispatch(new SetUserAdminAction(component:this,userDetails:await UC.GetUserFromUrl(e.Location)));
		});
	}
}