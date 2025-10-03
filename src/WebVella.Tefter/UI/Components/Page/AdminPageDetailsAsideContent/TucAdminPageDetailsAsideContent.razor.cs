namespace WebVella.Tefter.UI.Components;
public partial class TucAdminPageDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
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
			await _init(args.Payload);
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var pages = TfMetaService.GetAdminAddonPages(_search);

			_items = new();
			foreach (var page in pages)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminPagesSingleUrl, page.Id),
					Description = page.Description,
					Text = TfConverters.StringOverflow(page.Name, _stringLimit),
					Selected = navState.PageId == page.Id
				});
			}
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}
}