using Microsoft.JSInterop;

namespace WebVella.Tefter.Templates.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TemplatesAdminList.TemplatesAdminList", "WebVella.Tefter.Templates")]
public partial class TemplatesAdminList : TfBaseComponent
{
	[Inject] public ITemplatesService Service { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

	private List<Template> _templates = new();
	private List<ITemplateProcessor> _processors = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		if (TfAuxDataState.Value.Data.ContainsKey(TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY))
			_templates = (List<Template>)TfAuxDataState.Value.Data[TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY];
		if (TfAuxDataState.Value.Data.ContainsKey(TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY))
			_processors = (List<ITemplateProcessor>)TfAuxDataState.Value.Data[TemplatesConstants.TEMPLATE_APP_PROCESSORS_LIST_DATA_KEY];
	}

	private async Task _createTemplateHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TemplateCreateDialog>(
		new Template(),
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
			if (TfAuxDataState.Value.Data.ContainsKey(TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY))
				_templates = (List<Template>)TfAuxDataState.Value.Data[TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY];

			_templates.Add((Template)result.Data);
			TfAuxDataState.Value.Data[TemplatesConstants.TEMPLATE_APP_TEMPLATES_LIST_DATA_KEY] = _templates;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
			ToastService.ShowSuccess(LOC("Template successfully created!"));

		}
	}

	private async Task _editTemplateHandler(Template template)
	{
		//var dialog = await DialogService.ShowDialogAsync<TalkChannelManageDialog>(
		//channel,
		//new DialogParameters()
		//{
		//	PreventDismissOnOverlayClick = true,
		//	PreventScroll = true,
		//	Width = TfConstants.DialogWidthLarge,
		//	TrapFocus = false
		//});
		//var result = await dialog.Result;
		//if (!result.Cancelled && result.Data != null)
		//{
		//	var item = (TalkChannel)result.Data;
		//	List<TalkChannel> state = new();
		//	if (TfAuxDataState.Value.Data.ContainsKey(TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY))
		//		state = (List<TalkChannel>)TfAuxDataState.Value.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY];
		//	var itemIndex = state.FindIndex(x => x.Id == item.Id);
		//	if (itemIndex > -1)
		//	{
		//		state[itemIndex] = item;
		//	}
		//	TfAuxDataState.Value.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = state;
		//	Dispatcher.Dispatch(new SetAuxDataStateAction(
		//		component: this,
		//		state: TfAuxDataState.Value
		//	));
		//	ToastService.ShowSuccess(LOC("Channel successfully updated!"));

		//}
	}
	private async Task _deleteTemplateHandler(Template template)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this template deleted?")))
			return;
		//try
		//{
		//	var result = Service.DeleteChannel(channel.Id);
		//	ProcessServiceResponse(result);
		//	if (result.IsSuccess)
		//	{
		//		List<TalkChannel> state = new();
		//		if (TfAuxDataState.Value.Data.ContainsKey(TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY))
		//			state = (List<TalkChannel>)TfAuxDataState.Value.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY];
		//		var itemIndex = state.FindIndex(x => x.Id == channel.Id);
		//		if (itemIndex > -1)
		//		{
		//			state.RemoveAt(itemIndex);
		//		}
		//		TfAuxDataState.Value.Data[TalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = state;
		//		Dispatcher.Dispatch(new SetAuxDataStateAction(
		//			component: this,
		//			state: TfAuxDataState.Value
		//		));
		//	}
		//}
		//catch (Exception ex)
		//{
		//	ProcessException(ex);
		//}
		//finally
		//{
		//	await InvokeAsync(StateHasChanged);
		//}
	}

}