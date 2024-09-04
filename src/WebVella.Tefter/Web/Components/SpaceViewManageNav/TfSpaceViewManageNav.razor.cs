using RouteData = WebVella.Tefter.Web.Models.RouteData;

namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceViewManageNav.TfSpaceViewManageNav","WebVella.Tefter")]
public partial class TfSpaceViewManageNav : TfBaseComponent
{
	[Inject] protected IState<SpaceState> SpaceState { get; set; }

	private List<MenuItem> menu = new();

	private RouteData _urlData = new();

	protected override ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			Navigator.LocationChanged -= Navigator_LocationChanged;
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		return base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		GenerateMenu();
		ActionSubscriber.SubscribeToAction<SpaceStateChangedAction>(this, On_StateChanged);
		Navigator.LocationChanged += Navigator_LocationChanged;
		StateHasChanged();
	}

	private void GenerateMenu()
	{
		menu.Clear();
		var providerId = Navigator.GetUrlData().DataProviderId ?? Guid.Empty;
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.SpaceViewManagePageUrl, SpaceState.Value.RouteSpaceId, SpaceState.Value.RouteSpaceViewId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		_urlData = Navigator.GetUrlData();
	}


	private void On_StateChanged(SpaceStateChangedAction action)
	{
		GenerateMenu();
		StateHasChanged();
	}

	private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
	{
		GenerateMenu();
		StateHasChanged();
	}

	private async Task _editSpaceView()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceViewManageDialog>(
		SpaceState.Value.SpaceView,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceView)result.Data;
			var itemList = SpaceState.Value.SpaceViewList.ToList();
			var itemIndex = itemList.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				itemList[itemIndex] = item;
			}

			ToastService.ShowSuccess(LOC("Space view successfully updated!"));


			Dispatcher.Dispatch(new SetSpaceViewAction(
						spaceView: item,
						spaceViewList: itemList));
		}
	}
}