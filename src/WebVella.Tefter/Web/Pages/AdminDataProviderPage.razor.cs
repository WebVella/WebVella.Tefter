namespace WebVella.Tefter.Web.Pages;
public partial class AdminDataProviderPage : TfBasePage
{
	[Parameter] public Guid ProviderId { get; set; }
	[Parameter] public string Path { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (ProviderId == Guid.Empty && TfAppState.Value.AdminDataProviders is not null
			&& TfAppState.Value.AdminDataProviders.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataProviderDetailsPageUrl, TfAppState.Value.AdminDataProviders[0].Id));
		}
	}
}