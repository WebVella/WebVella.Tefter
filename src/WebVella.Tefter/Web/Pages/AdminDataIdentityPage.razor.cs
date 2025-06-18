namespace WebVella.Tefter.Web.Pages;
public partial class AdminDataIdentityPage : TfBasePage
{
	[Parameter] public string DataIdentityId { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (String.IsNullOrWhiteSpace(DataIdentityId) && TfAppState.Value.AdminDataIdentities is not null
			&& TfAppState.Value.AdminDataIdentities.Count > 0)
		{
			Navigator.NavigateTo(string.Format(TfConstants.AdminDataIdentityDetailsPageUrl, TfAppState.Value.AdminDataIdentities[0].Name));
		}
	}
}