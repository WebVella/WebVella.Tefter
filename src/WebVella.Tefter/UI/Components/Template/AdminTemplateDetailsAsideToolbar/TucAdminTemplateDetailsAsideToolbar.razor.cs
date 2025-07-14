namespace WebVella.Tefter.UI.Components;
public partial class TucAdminTemplateDetailsAsideToolbar : TfBaseComponent
{
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;
	private string? _search = null;
	private TfNavigationState _navState = default!;

	public void Dispose()
	{
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		_search = NavigatorExt.GetStringFromQuery(Navigator,TfConstants.AsideSearchQueryName,null);
		_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}

	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		_navState = args;
		await InvokeAsync(StateHasChanged);
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