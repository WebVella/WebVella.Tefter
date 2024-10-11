

namespace WebVella.Tefter.Web.Components;

[LocalizationResource("WebVella.Tefter.Talk.Components.TalkThreadPanel.TalkThreadPanel", "WebVella.Tefter.Talk")]
public partial class TalkThreadPanel : TfFormBaseComponent, IDialogContentComponent<TalkThreadPanelContext>
{
	[Inject] public IState<TfAppState> TfAppState { get; set; }
	[Inject] public IState<TfUserState> TfUserState { get; set; }
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkThreadPanelContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isLoading = true;

	private TfEditor _channelEditor = null;
	private string _channelEditorContent = null;
	private bool _channelEditorSending = false;

	private TfEditor _threadEditor = null;
	private string _threadEditorContent = null;
	private bool _threadEditorSending = false;

	private TalkThread _activeThread = null;
	private TalkChannel _channel = null;
	private Guid? _skValue = null;
	private Guid _rowId = Guid.Empty;
	private List<TalkThread> _threads = new();

	private Guid? _threadEditedId = null;
	private Guid? _subthreadEditedId = null;
	private Guid? _threadIdUpdateSaving = null;
	private bool _threadVisibleInChannel = false;
	private bool _threadBroadcastVisible = false;
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Content.ChannelId is not null)
			{
				var getChannelResult = TalkService.GetChannel(Content.ChannelId.Value);
				if (getChannelResult.IsSuccess) _channel = getChannelResult.Value;
				else throw new Exception("GetChannel failed");
				if (_channel is not null && !String.IsNullOrWhiteSpace(_channel.SharedKey) && Content.RowIndex > -1)
				{
					_rowId = (Guid)Content.DataTable.Rows[Content.RowIndex][TfConstants.TEFTER_ITEM_ID_PROP_NAME];
					_skValue = Content.DataTable.Rows[Content.RowIndex].GetSharedKeyValue(_channel.SharedKey);
					if (_skValue is not null)
					{
						var getThreadsResult = TalkService.GetThreads(_channel.Id, _skValue);
						if (getThreadsResult.IsSuccess) _threads = getThreadsResult.Value;
						else throw new Exception("GetThreads failed");
					}
				}
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
				await Task.Delay(100);
				if(_channelEditor is not null)
					await _channelEditor.Focus();
			}
			else
			{
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
			}

		}

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _sendMessage()
	{
		Console.WriteLine("_sendMessage");
		if (_channelEditorSending) return;
		_channelEditorSending = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var submit = new CreateTalkThread
			{
				ChannelId = _channel.Id,
				Content = _channelEditorContent,
				Type = TalkThreadType.Comment,
				UserId = TfUserState.Value.CurrentUser.Id,
				DataProviderId = Content.DataTable.QueryInfo.DataProviderId,
				RowIds = new List<Guid> { _rowId }
			};
			var result = TalkService.CreateThread(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message is sent"));
				_channelEditorContent = null;
				var getThreadResult = TalkService.GetThread(result.Value);
				if (getThreadResult.IsFailed) throw new Exception("GetThreads failed");
				_threads.Insert(0, getThreadResult.Value);

			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_channelEditorSending = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _replyMessage()
	{
		if (_threadEditorSending) return;
		_threadEditorSending = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var submit = new CreateTalkSubThread
			{
				VisibleInChannel = _threadVisibleInChannel,
				Content = _threadEditorContent,
				ThreadId = _activeThread.Id,
				UserId = TfUserState.Value.CurrentUser.Id,
			};
			var result = TalkService.CreateSubThread(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message is sent"));
				_threadEditorContent = null;
				var getThreadResult = TalkService.GetThread(_activeThread.Id);
				if (getThreadResult.IsSuccess)
				{
					_activeThread = getThreadResult.Value;
					var threadIndex = _threads.FindIndex(x => x.Id == _activeThread.Id);
					if (threadIndex > -1) _threads[threadIndex] = _activeThread;
					if (_threadVisibleInChannel)
					{
						var subThreadIndex = _activeThread.SubThread.FindIndex(x => x.Id == result.Value);
						if (subThreadIndex > -1)
						{
							_threads.Insert(0, _activeThread.SubThread[subThreadIndex]);
						}
					}
				}
				else throw new Exception("GetThread failed");
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_threadEditorSending = false;
			_threadVisibleInChannel = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private Dictionary<Guid, string> _generateThreadClassDict(List<TalkThread> threads)
	{
		var result = new Dictionary<Guid, string>();
		int mainIndex = 0;
		var threadsReversed = threads.OrderBy(x => x.CreatedOn).ToList();
		foreach (var message in threadsReversed)
		{
			var isFirst = mainIndex == 0;
			var isLast = mainIndex == threadsReversed.Count - 1;
			var prevMain = isFirst ? null : threadsReversed[mainIndex - 1];
			var nextMain = isLast ? null : threadsReversed[mainIndex + 1];
			var cssList = new List<string>();
			if (
				prevMain is not null && !prevMain.DeletedOn.HasValue
				&& prevMain.User.Id == message.User.Id
				&& !message.DeletedOn.HasValue
				&& !(message.VisibleInChannel && message.ThreadId.HasValue)
				&& (message.RelatedSK is null || message.RelatedSK.Count <= 1)
				&& (message.CreatedOn - prevMain.CreatedOn).TotalMinutes <= 5)
			{
				cssList.Add("talk-followup");
			}

			result[message.Id] = String.Join(" ", cssList);
			mainIndex++;
		}
		return result;
	}

	private void _replyToThread(TalkThread thread)
	{
		_activeThread = _activeThread?.Id != thread.Id ? thread : null;
	}
	private void _onSubThreadView(TalkThread thread)
	{
		_activeThread = thread.ParentThread;
	}


	private void _closeActiveThread()
	{
		_activeThread = null;
	}

	private async Task _editThread(TalkThread thread, bool isChannelThread = true)
	{
		if (isChannelThread)
		{
			if (_threadEditedId is not null)
			{
				if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("You will loose any unsaved changes on your previous edit. Do you want to continue?")))
					return;
			}

			if (_threadEditedId == thread.Id) _threadEditedId = null;
			else _threadEditedId = thread.Id;
		}
		else
		{
			if (_subthreadEditedId is not null)
			{
				if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("You will loose any unsaved changes on your previous edit. Do you want to continue?")))
					return;
			}

			if (_subthreadEditedId == thread.Id) _subthreadEditedId = null;
			else _subthreadEditedId = thread.Id;
		}
	}

	private async Task _deleteThread(TalkThread thread)
	{

		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this thread deleted?")))
			return;

		try
		{
			var result = TalkService.DeleteThread(thread.Id);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message deleted"));
				int threadsIndex = _threads.FindIndex(x => x.Id == thread.Id);
				int subthreadsIndex = _activeThread is null ? -1 : _activeThread.SubThread.FindIndex(x => x.Id == thread.Id);
				var now = DateTime.Now;
				if (threadsIndex > -1) _threads[threadsIndex].DeletedOn = now;
				if (subthreadsIndex > -1) _activeThread.SubThread[subthreadsIndex].DeletedOn = now;
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

	private async Task _saveMessage(TalkThread thread, string content)
	{
		if (_threadIdUpdateSaving is not null) return;
		_threadIdUpdateSaving = thread.Id;
		await InvokeAsync(StateHasChanged);
		try
		{
			var result = TalkService.UpdateThread(thread.Id, content);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message saved"));
				int threadsIndex = _threads.FindIndex(x => x.Id == thread.Id);
				int subthreadsIndex = _activeThread is null ? -1 : _activeThread.SubThread.FindIndex(x => x.Id == thread.Id);
				var now = DateTime.Now;
				if (threadsIndex > -1) _threads[threadsIndex].Content = content;
				if (subthreadsIndex > -1) _activeThread.SubThread[subthreadsIndex].Content = content;
				_threadEditedId = null;
				_subthreadEditedId = null;
				thread.Content = content;
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_threadIdUpdateSaving = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private Task _cancelSaveMessage()
	{

		_threadEditedId = null;
		return Task.CompletedTask;
	}

	private Task _showThreadBradcastDetails()
	{
		_threadBroadcastVisible = !_threadBroadcastVisible;
		return Task.CompletedTask;
	}
}

public record TalkThreadPanelContext
{
	public Guid? ChannelId { get; set; }
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;

}
