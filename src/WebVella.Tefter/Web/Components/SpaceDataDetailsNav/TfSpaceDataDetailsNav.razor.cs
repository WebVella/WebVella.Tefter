using RouteData = WebVella.Tefter.Web.Models.RouteData;

namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.SpaceDataDetailsNav.TfSpaceDataDetailsNav","WebVella.Tefter")]
public partial class TfSpaceDataDetailsNav : TfBaseComponent
{
	[Inject] protected IState<TfState> TfState { get; set; }

	private List<MenuItem> menu = new();
	private List<TucSpaceView> viewList = new();

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
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfState.Value.RouteSpaceId, TfState.Value.RouteSpaceDataId),
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Info(),
			Title = LOC("Details")
		});
		menu.Add(new MenuItem
		{
			Url = String.Format(TfConstants.SpaceDataPageUrl, TfState.Value.RouteSpaceId, TfState.Value.RouteSpaceDataId) + "\\views",
			Match = NavLinkMatch.All,
			//Icon = new Icons.Regular.Size20.Table(),
			Title = LOC("Used in Views")
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

	private async Task _editSpaceData()
	{

		var dialog = await DialogService.ShowDialogAsync<TfSpaceDataManageDialog>(
		TfState.Value.SpaceData,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
			var item = (TucSpaceData)result.Data;
			var itemList = TfState.Value.SpaceDataList.ToList();
			var itemIndex = itemList.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				itemList[itemIndex] = item;
			}

			ToastService.ShowSuccess(LOC("Space dataset successfully updated!"));


			Dispatcher.Dispatch(new SetSpaceDataAction(
						component:this,
						spaceData: item,
						spaceDataList: itemList));
		}
	}
}