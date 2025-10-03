namespace WebVella.Tefter.UI.Components;
public partial class TucAdminSharedColumnDetailsAsideContent : TfBaseComponent, IDisposable
{
	private bool _isLoading = true;
	private int _stringLimit = 30;
	private string? _search = String.Empty;
	private List<TfMenuItem> _items = new();
	public void Dispose()
	{
		TfEventProvider.SharedColumnCreatedEvent -= On_SharedColumnChanged;
		TfEventProvider.SharedColumnUpdatedEvent -= On_SharedColumnChanged;
		TfEventProvider.SharedColumnDeletedEvent -= On_SharedColumnChanged;
		TfAuthLayout.NavigationStateChangedEvent -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await _init(TfAuthLayout.NavigationState);
		TfEventProvider.SharedColumnCreatedEvent += On_SharedColumnChanged;
		TfEventProvider.SharedColumnUpdatedEvent += On_SharedColumnChanged;
		TfEventProvider.SharedColumnDeletedEvent += On_SharedColumnChanged;
		TfAuthLayout.NavigationStateChangedEvent += On_NavigationStateChanged;
	}

	private async void On_SharedColumnChanged(object args)
	{
		await InvokeAsync(async () =>
		{
			await _init(TfAuthLayout.NavigationState);
		});
	}


	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		await InvokeAsync(async () =>
		{
			if (UriInitialized != args.Uri)
				await _init(args);
		});
	}

	private async Task _init(TfNavigationState navState)
	{
		try
		{
			_search = navState.SearchAside;
			var items = TfService.GetSharedColumns(_search).ToList();
			_items = new();
			foreach (var item in items)
			{
				_items.Add(new TfMenuItem
				{
					Url = string.Format(TfConstants.AdminSharedColumnDetailsPageUrl, item.Id),
					Description = item.DataIdentity,
					Text = TfConverters.StringOverflow(item.DbName, _stringLimit),
					Selected = navState.SharedColumnId == item.Id
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