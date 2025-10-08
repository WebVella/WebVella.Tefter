namespace WebVella.Tefter.UI.Components;
public partial class TucAdminPageDetailsContent : TfBaseComponent, IDisposable
{
	private TfScreenRegionComponentMeta? _item = null;
	public void Dispose()
	{
		TfState.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfState.NavigationState);
		TfState.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async Task On_NavigationStateChanged(TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(navState: args);
		});
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