namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceData.SpaceDataJoinData.TfSpaceDataJoinData", "WebVella.Tefter")]
public partial class TfSpaceDataJoinData : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private TucDataProvider _dataProvider = new();
	private List<TucDataProvider> _joinedProviders = new();

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _generateItems();
	}
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}

	private async void On_AppChanged(SetAppStateAction action)
	{
		await InvokeAsync(async () =>
		{
			if (TfAppState.Value.SpaceData is not null)
			{
				await _generateItems();
				StateHasChanged();
			}
		});
	}

	private async Task _generateItems()
	{
		_joinedProviders = await UC.GetDataProviderJoinedProvidersAsync(TfAppState.Value.SpaceData.DataProviderId);
		_dataProvider = await UC.GetDataProviderAsync(TfAppState.Value.SpaceData.DataProviderId);
	}

	private string _showCommonIdentities(TucDataProvider subProvider)
	{
		var commonIdentities = _dataProvider.Identities
							.Select(x => x.Name)
							.Intersect(subProvider.Identities.Select(x => x.Name)).ToList();
		if (commonIdentities.Count == 0) return "n/a";
		return String.Join(", ", commonIdentities);
	}
}
