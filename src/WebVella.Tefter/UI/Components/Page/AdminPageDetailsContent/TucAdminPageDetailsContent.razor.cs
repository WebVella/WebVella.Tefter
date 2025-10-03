namespace WebVella.Tefter.UI.Components;
public partial class TucAdminPageDetailsContent : TfBaseComponent, IDisposable
{
	private TfScreenRegionComponentMeta? _item = null;
	public void Dispose()
	{
		TfEventProvider.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(TfNavigationStateChangedEvent args)
	{
		if (args.IsUserApplicable(TfAuthLayout.CurrentUser) && UriInitialized != args.Payload.Uri)
			await _init(navState: args.Payload);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_item = null;
			if (navState.PageId is not null)
			{
				var pages = TfMetaService.GetAdminAddonPages();
				_item = pages.FirstOrDefault(x => x.Id == navState.PageId);
			}
		}
		finally
		{
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}