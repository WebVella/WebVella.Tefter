namespace WebVella.Tefter.UI.Components;
public partial class TucSpaceHeader : TfBaseComponent, IDisposable
{
	[Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;

	private TfSpace? _space = null!;
	private bool _spacesActive = false;
	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
		TfEventProvider.Dispose();
	}
	protected override void OnInitialized()
	{
		_space = TfAuthLayout.GetState().Space;
		Navigator.LocationChanged += On_NavigationStateChanged;
		TfEventProvider.SpaceUpdatedEvent += On_SpaceUpdated;
	}
	
	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		_space = TfAuthLayout.GetState().Space;
		StateHasChanged();
	}	
	
	private async Task On_SpaceUpdated(TfSpaceUpdatedEvent args)
	{
		_space = args.Payload;
		await InvokeAsync(StateHasChanged);
	}

	private void _manageCurrentSpace()
	{
		var pageManageUrl = String.Format(TfConstants.SpaceManagePageUrl, TfAuthLayout.GetState().Space!.Id);
		Navigator.NavigateTo(pageManageUrl.GenerateWithLocalAndQueryAsReturnUrl(Navigator.Uri)!);
	}


	private async Task _deleteSpace()
	{
		var state = TfAuthLayout.GetState();
		if (state.Space is null) return;
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this space deleted?")))
			return;
		try
		{
			TfService.DeleteSpace(state.Space.Id);
			ToastService.ShowSuccess(LOC("Space deleted"));
			Navigator.NavigateTo(TfConstants.HomePageUrl);
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
	}
}
