namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminTemplateNavigation.TfAdminTemplateNavigation", "WebVella.Tefter")]
public partial class TfAdminTemplateNavigation : TfBaseComponent, IAsyncDisposable
{
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);

	}

	private async Task onAddClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateCreateDialog>(
		new TucTemplate(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var user = (TucUser)result.Data;
			ToastService.ShowSuccess(LOC("Template successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.AdminUserDetailsPageUrl, user.Id));
		}
	}


}