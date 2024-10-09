namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkThreadModal.TalkThreadModal", "WebVella.Tefter.Talk")]
public partial class TalkThreadModal : TfFormBaseComponent, IDialogContentComponent<TalkThreadModalContext>
{
	[Inject] public IState<TfUserState> TfUserState { get; set; }
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkThreadModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = string.Empty;
	private string _error = string.Empty;
	private bool _isLoading = true;
	private bool _isSubmitting = false;
	private TfEditor _editor = null;
	private List<TalkChannel> _channels = new();
	private TalkChannel _selectedChannel = null;
	private string _content = null;
	private List<Guid> _sharedKeyValueIds = new();



	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Content is null) return;
			//if (Content.DataTable.Rows.Count > 0)
			//{
			//	foreach (var tfId in Content.SelectedRowIds)
			//	{
					
			//	}


			//	var allChannels = new List<TalkChannel>();
			//	var getChannelsResult = TalkService.GetChannels();
			//	if (getChannelsResult.IsSuccess) allChannels = getChannelsResult.Value;
			//	else throw new Exception("GetChannels failed");

			//	//Select only channels that are compatible with this DataProvider
			//	foreach (var channel in allChannels)
			//	{
			//		if (String.IsNullOrWhiteSpace(channel.SharedKey)) continue;
			//		var keyValue = Content.DataTable.Rows[0].GetSharedKeyValue(channel.SharedKey);
			//		if (keyValue is null) continue;
			//		_channels.Add(channel);
			//	}

			//	if (_channels.Count == 1)
			//	{
			//		await _selectChannelHandler(_channels[0]);
			//	}
			//	else if (_channels.Count > 1)
			//	{
			//		_title = LOC("Select a channel");
			//	}

			//}
			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		_isSubmitting = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var submit = new CreateTalkThread
			{
				VisibleInChannel = false,
				ChannelId = _selectedChannel.Id,
				Content = _content,
				ThreadId = null,
				Type = TalkThreadType.Comment,
				UserId = TfUserState.Value.CurrentUser.Id,
				RowIds = Content.SelectedRowIds,
				DataProviderId = Content.DataProviderId
			};
			var result = TalkService.CreateThread(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message is sent"));
				_content = null;
				await _cancel();
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _selectChannelHandler(TalkChannel channel)
	{
		_selectedChannel = channel;
		_title = $"#{channel.Name}";
		await Task.Delay(100);
		if (_editor is not null)
			await _editor.Focus();
	}
}

public record TalkThreadModalContext
{
	public Guid DataProviderId { get; set; }
	public List<Guid> SelectedRowIds { get; set; } = new();

}