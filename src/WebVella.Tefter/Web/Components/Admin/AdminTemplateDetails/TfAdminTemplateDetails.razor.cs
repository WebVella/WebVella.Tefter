
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.AdminTemplateDetails.TfAdminTemplateDetails", "WebVella.Tefter")]
public partial class TfAdminTemplateDetails : TfBaseComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	private ITfTemplateProcessor _processor = null;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_processor = _getProcessor();
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
			Dispatcher.Dispatch(new SetAppStateAction(component: this,
				state: TfAppState.Value with { AdminTemplateDetails = template }));
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
		dict["Value"] = TfAppState.Value.AdminTemplateDetails?.SettingsJson;
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
}