namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Navigation.TfNavigation","WebVella.Tefter")]
public partial class TfNavigation : TfBaseComponent
{
	[Inject] protected IState<TfState> TfState { get; set; }
	[Inject] protected IStateSelection<TfState, bool> ScreenStateSidebarExpanded { get; set; }
	private List<MenuItem> SpaceNav { get; set; } = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
			Navigator.LocationChanged -= Navigator_LocationChanged;
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		ScreenStateSidebarExpanded.Select(x => x?.SidebarExpanded ?? true);
		_generateSpaceNav();
		Navigator.LocationChanged += Navigator_LocationChanged;
	}

	protected void Navigator_LocationChanged(object sender, LocationChangedEventArgs args)
	{

		base.InvokeAsync(async () =>
		{
			_generateSpaceNav();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _generateSpaceNav()
	{
		SpaceNav.Clear();
		if (TfState.Value.CurrentUserSpaces is null) return;
		foreach (var item in TfState.Value.CurrentUserSpaces)
		{
			SpaceNav.Add(new MenuItem
			{
				Icon = item.Icon,
				Id = RenderUtils.ConvertGuidToHtmlElementId(item.Id),
				Match = NavLinkMatch.Prefix,
				Url = String.Format(TfConstants.SpacePageUrl, item.Id), //item.DefaultViewId - active menu issues
				Title = item.Name,
				IconColor = item.Color,
			});
		}
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

}