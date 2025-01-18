
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminTemplateDetails.TfAdminTemplateDetails", "WebVella.Tefter")]
public partial class TfAdminTemplateDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private ITfTemplateProcessor _processor = null;
	private List<TfSpaceDataAsOption> _spaceDataAll = new();
	private List<TfSpaceDataAsOption> _spaceDataSelection = new();
	private bool _loading = true;
	private DynamicComponent _settingsComponent;
	private TfTemplateProcessorSettingsComponentContext _setttingsContext = null;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_processor = _getProcessor();
		_setttingsContext = new TfTemplateProcessorSettingsComponentContext
		{
			Template = TfAppState.Value.AdminTemplateDetails,
			Validate = null
		};
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_spaceDataAll = UC.GetSpaceDataOptionsForTemplate();
			_recalcSpaceDataOptions();
			_loading = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task onUpdateClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateManageDialog>(
		TfAppState.Value.AdminTemplateDetails,
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
			ToastService.ShowSuccess(LOC("Template successfully updated!"));
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminTemplateDetails = template }));
			_recalcSpaceDataOptions();
		}
	}

	private async Task onUpdateSettingsClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateSettingsDialog>(
		TfAppState.Value.AdminTemplateDetails,
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
			ToastService.ShowSuccess(LOC("Template successfully updated!"));
			_setttingsContext.Template = template;
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminTemplateDetails = template }));

			_recalcSpaceDataOptions();
		}
	}

	private async Task onHelpClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateHelpDialog>(
		_processor,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Read;
		dict["Context"] = _setttingsContext;
		return dict;
	}

	private ITfTemplateProcessor _getProcessor()
	{
		var context = TfAppState.Value.AdminTemplateDetails;
		if (context is null) return null;
		if (context.ContentProcessorType is not null && context.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			return (ITfTemplateProcessor)Activator.CreateInstance(context.ContentProcessorType);

		}
		return null;

	}

	private void _recalcSpaceDataOptions()
	{
		_spaceDataSelection = new();
		foreach (var item in TfAppState.Value.AdminTemplateDetails.SpaceDataList)
		{
			var attachment = _spaceDataAll.Where(x => x.Id == item).FirstOrDefault();
			if (attachment is null) continue;
			_spaceDataSelection.Add(attachment);
		}
	}
}