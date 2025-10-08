namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideToolbar : TfBaseComponent
{
	private string? _search = null;
	private TfNavigationState _navState = null!;

	public void Dispose()
	{
		Navigator.LocationChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		_search = NavigatorExt.GetStringFromQuery(Navigator,TfConstants.AsideSearchQueryName,null);
		_navState = TfAuthLayout.GetState().NavigationState;
		Navigator.LocationChanged += On_NavigationStateChanged;
	}

	private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
	{
		InvokeAsync(async () =>
		{
			_navState = TfAuthLayout.GetState().NavigationState;
			await InvokeAsync(StateHasChanged);
		});
	}

	private async Task onSearch(string search)
	{
		_search = search;
		await NavigatorExt.ApplyChangeToUrlQuery(Navigator,TfConstants.AsideSearchQueryName,search);
	}

	private async Task addTemplate()
	{
		var dialog = await DialogService.ShowDialogAsync<TucTemplateManageDialog>(
		new TfTemplate(){ 
			ResultType = _navState.TemplateResultType ?? TfTemplateResultType.File
		},
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
			var item = (TfTemplate)result.Data;
			Navigator.NavigateTo(string.Format(TfConstants.AdminTemplatesTemplatePageUrl, (int)item.ResultType,item.Id));
		}

	}
}