namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminTemplates.TfAdminTemplates", "WebVella.Tefter")]
public partial class TfAdminTemplates : TfBaseComponent
{
	[Inject] private AppStateUseCase UC { get; set; }
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private string _search = null;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_search = TfAppState.Value.Route.Search;
	}

	private async Task _addTemplate()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateManageDialog>(
		new TucTemplate() { ResultType = TfAppState.Value.Route.TemplateResultType ?? TfTemplateResultType.File},
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
			var template = (TucTemplate)result.Data;
			ToastService.ShowSuccess(LOC("Template successfully created!"));
			Navigator.NavigateTo(String.Format(TfConstants.AdminTemplatesTemplatePageUrl, (int)TfAppState.Value.Route.TemplateResultType,template.Id));
		}
	}

	private async Task _deleteColumn(TucTemplate template)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this template deleted?")))
			return;
		try
		{
			Result result = UC.DeleteTemplate(template.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("The template was successfully deleted!"));
				var templates = TfAppState.Value.AdminTemplateList.Where(x=> x.Id != template.Id).ToList();
				Dispatcher.Dispatch(new SetAppStateAction(component: this,
					state: TfAppState.Value with { AdminTemplateList = templates }));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}

	}

	private async Task _searchValueChanged(string search)
	{
		_search = search?.Trim();
		await InvokeAsync(StateHasChanged);
	}

	private List<TucTemplate> _getTemplates()
	{
		string searchProcessed = null;
		if (!String.IsNullOrWhiteSpace(_search))
			searchProcessed = _search.Trim().ToLowerInvariant();

		if (!String.IsNullOrWhiteSpace(searchProcessed))
		{
			return TfAppState.Value.AdminTemplateList.Where(x=> 
				x.Name.ToLowerInvariant().Contains(searchProcessed)
				|| x.UsedColumns.Any(z=> z.ToLowerInvariant().Contains(searchProcessed))
			).ToList();
		}
		else
		{
			return TfAppState.Value.AdminTemplateList;
		}

	}
}