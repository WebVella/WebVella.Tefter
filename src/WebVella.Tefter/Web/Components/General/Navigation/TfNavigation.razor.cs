namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.General.Navigation.TfNavigation", "WebVella.Tefter")]
public partial class TfNavigation : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }

	private List<TucMenuItem> _topMenuItems = new();
	private List<TucMenuItem> _mainMenuItems = new();
	private OfficeColor _defaultColor = TfConstants.DefaultThemeColor;

	protected override async ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing)
		{
			ActionSubscriber.UnsubscribeFromAllActions(this);
		}
		await base.DisposeAsyncCore(disposing);
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_defaultColor = TfAppState.Value.CurrentUser.Settings?.ThemeColor ?? TfConstants.DefaultThemeColor;
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			_initMenus();
			StateHasChanged();
			ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
		}
	}
	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_initMenus();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _initMenus()
	{
		_topMenuItems.Clear();
		_mainMenuItems.Clear();

		var uri = new Uri(Navigator.Uri);
		//Top
		_topMenuItems.Add(new TucMenuItem()
		{
			Id = TfConverters.ConvertGuidToHtmlElementId(new Guid("b79b0776-0587-46d0-85bf-32b998aec4e8")),
			IconExpanded = TfConstants.HomeIcon.WithColor(_defaultColor.ToAttributeValue()),
			IconCollapsed = TfConstants.HomeIcon.WithColor(_defaultColor.ToAttributeValue()),
			IconColor = _defaultColor,
			Text = TfConstants.HomeMenuTitle,
			Url = "/",
			Selected = uri.LocalPath == "/"
		});
		if (TfAppState.Value.Pages.Count > 0)
		{
			_topMenuItems.Add(new TucMenuItem()
			{
				Id = TfConverters.ConvertGuidToHtmlElementId(new Guid("5277cf88-d852-4af6-b551-7650fca3a596")),
				IconExpanded = TfConstants.ApplicationIcon.WithColor(_defaultColor.ToAttributeValue()),
				IconCollapsed = TfConstants.ApplicationIcon.WithColor(_defaultColor.ToAttributeValue()),
				IconColor = _defaultColor,
				Text = TfConstants.PagesMenuTitle,
				Url = String.Format(TfConstants.PagesSinglePageUrl, TfAppState.Value.Pages[0].Slug),
				Selected = uri.LocalPath.StartsWith(TfConstants.PagesPageUrl)
			});
		}

		//Main
		foreach (var item in TfAppState.Value.SpacesNav)
		{
			_mainMenuItems.Add(new TucMenuItem()
			{
				Id = item.Id,
				IconCollapsed = item.IconCollapsed.WithColor(item.IconColor.ToAttributeValue()),
				IconExpanded = item.IconExpanded.WithColor(item.IconColor.ToAttributeValue()),
				IconColor = item.IconColor,
				Text = item.Text,
				Url = item.Url,
				Selected = uri.LocalPath.StartsWith(String.Format(TfConstants.SpacePageUrl, TfConverters.ConvertHtmlElementIdToGuid(item.Id)))
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