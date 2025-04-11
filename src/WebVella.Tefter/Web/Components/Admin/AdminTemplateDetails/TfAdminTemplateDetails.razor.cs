
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminTemplateDetails.TfAdminTemplateDetails", "WebVella.Tefter")]
public partial class TfAdminTemplateDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	private ITfTemplateProcessor _processor = null;
	private List<TfSpaceDataAsOption> _spaceDataAll = new();
	private List<TfSpaceDataAsOption> _spaceDataSelection = new();

	private TfTemplateProcessorDisplaySettingsScreenRegionContext _dynamicComponentContext = null;
	private TfScreenRegionScope _dynamicComponentScope = null;

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
		_initDynamicComponent();
		ActionSubscriber.SubscribeToAction<SetAppStateAction>(this, On_AppChanged);
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			_spaceDataAll = UC.GetSpaceDataOptionsForTemplate();
			_recalcSpaceDataOptions();
			await InvokeAsync(StateHasChanged);
		}

	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		_initDynamicComponent();
	}

	private void On_AppChanged(SetAppStateAction action)
	{
		InvokeAsync(async () =>
		{
			_initDynamicComponent();
			await InvokeAsync(StateHasChanged);
		});
	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();

		if (TfAppState.Value.AdminTemplateDetails is null)
			throw new Exception("Template not found");

		_dynamicComponentContext = new TfTemplateProcessorDisplaySettingsScreenRegionContext
		{
			Template = TfAppState.Value.AdminTemplateDetails
		};
		_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(),null);
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
			//_setttingsContext.Template = template;
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminTemplateDetails = template }));

			_recalcSpaceDataOptions();
		}
	}

	private async Task onHelpClick()
	{
		var dialog = await DialogService.ShowDialogAsync<TfTemplateHelpDialog>(
		TfAppState.Value.AdminTemplateDetails,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
	}


	private ITfTemplateProcessor _getProcessor()
	{
		if (TfAppState.Value.AdminTemplateDetails is null)
			throw new Exception("Template not found");

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