namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Navigation.TfNavigation", "WebVella.Tefter")]
public partial class TfNavigation : TfBaseComponent
{
	[Inject] protected IStateSelection<TfUserState,bool> SidebarExpanded { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		SidebarExpanded.Select(x => x.SidebarExpanded);
	}
	private async Task _addSpaceHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSpaceManageDialog>(
		new TucSpace(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpace)result.Data;
			ToastService.ShowSuccess(LOC("Space successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.SpacePageUrl, item.Id));
		}
	}

	private async Task _searchSpaceHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TfSearchSpaceDialog>(
		true,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
	}

	private string _spaceSelectedClass(TucMenuItem menu)
	{
		if (String.IsNullOrWhiteSpace(menu.Id)) return "";
		var uri = new Uri(Navigator.Uri);
		var spaceId = TfConverters.ConvertHtmlElementIdToGuid(menu.Id);
		if (uri.LocalPath.StartsWith(String.Format(TfConstants.SpacePageUrl, spaceId.Value)))
			return "selected";

		return "";
	}
}