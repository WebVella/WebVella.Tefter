using Microsoft.JSInterop;

namespace WebVella.Tefter.Talk.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkChannelAdminList.TalkChannelAdminList", "WebVella.Tefter.Talk")]
public partial class TalkChannelAdminList : TfBaseComponent
{
	[Inject] public ITalkService TalkService { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }

	private async Task _createChannelHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<TalkChannelManageDialog>(
		new TalkChannel(),
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
			List<TalkChannel> state = new();
			if (TfAuxDataState.Value.Data.ContainsKey(TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY))
				state = (List<TalkChannel>)TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY];

			state.Add((TalkChannel)result.Data);
			TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = state;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
			ToastService.ShowSuccess(LOC("Channel successfully created!"));

		}
	}

	private async Task _editChannelHandler(TalkChannel channel)
	{
		var dialog = await DialogService.ShowDialogAsync<TalkChannelManageDialog>(
		channel,
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
			var item = (TalkChannel)result.Data;
			List<TalkChannel> state = new();
			if (TfAuxDataState.Value.Data.ContainsKey(TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY))
				state = (List<TalkChannel>)TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY];
			var itemIndex = state.FindIndex(x => x.Id == item.Id);
			if (itemIndex > -1)
			{
				state[itemIndex] = item;
			}
			TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = state;
			Dispatcher.Dispatch(new SetAuxDataStateAction(
				component: this,
				state: TfAuxDataState.Value
			));
			ToastService.ShowSuccess(LOC("Channel successfully updated!"));

		}
	}
	private async Task _deleteChannelHandler(TalkChannel channel)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this channel deleted? This will delete all threads and comments in it!")))
			return;
		try
		{
			var result = TalkService.DeleteChannel(channel.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				List<TalkChannel> state = new();
				if (TfAuxDataState.Value.Data.ContainsKey(TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY))
					state = (List<TalkChannel>)TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY];
				var itemIndex = state.FindIndex(x => x.Id == channel.Id);
				if (itemIndex > -1)
				{
					state.RemoveAt(itemIndex);
				}
				TfAuxDataState.Value.Data[TfTalkConstants.TALK_APP_CHANNEL_LIST_DATA_KEY] = state;
				Dispatcher.Dispatch(new SetAuxDataStateAction(
					component: this,
					state: TfAuxDataState.Value
				));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			await InvokeAsync(StateHasChanged);
		}
	}

}