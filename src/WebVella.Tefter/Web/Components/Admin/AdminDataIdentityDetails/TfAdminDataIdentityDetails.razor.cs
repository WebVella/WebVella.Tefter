namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminDataIdentityDetails.TfAdminDataIdentityDetails", "WebVella.Tefter")]
public partial class TfAdminDataIdentityDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }

	internal List<TucDataProvider> _dataProviders = new();
	
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
		_dataProviders = await UC.GetDataProvidersImplementingIdentity(TfAppState.Value.AdminDataIdentity?.Name);
		ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_dataProviders = await UC.GetDataProvidersImplementingIdentity(TfAppState.Value.AdminDataIdentity?.Name);
			await InvokeAsync(StateHasChanged);
		});
	}

	private string _getProviderImplementation(TucDataProvider provider){ 
		if(provider is null || provider.Identities is null || provider.Identities.Count == 0)
			return null;

		var implementation = provider.Identities.FirstOrDefault(x=> x.Name == TfAppState.Value.AdminDataIdentity?.Name);
		if(implementation == null) 
			return null;

		return String.Join(", ", implementation.Columns);
	}
}