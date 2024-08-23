using WebVella.Tefter.Web.Components.SpaceManageDialog;

namespace WebVella.Tefter.Web.Components.Navigation;
public partial class TfNavigation: TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }
	[Inject] protected IStateSelection<ScreenState, bool> ScreenStateSidebarExpanded { get; set; }
	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
	}
	private async Task _addSpaceHandler(){
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

}