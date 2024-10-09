

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
	private bool _primarySending = false;

	private TfEditor _mainEditor = null;
	private TfEditor _mainEditEditor = null;
	private TfEditor _subEditor = null;
	private Guid? _activeThreadId = null;
	private TalkChannel _channel = null;
	private Guid? _skValue = null;
	private Guid _rowId = Guid.Empty;
	private List<TalkThread> _threads = new();
	private string _primaryContent = null;
	private Dictionary<Guid, string> _threadClassDict = new();
	private Guid? _threadEditedId = null;
	private Guid? _threadIdUpdateSaving = null;
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
						_generateThreadClassDict();
					}
				}
				_isLoading = false;
				await InvokeAsync(StateHasChanged);
				await Task.Delay(100);
				await _mainEditor.Focus();
			}

		}

	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _sendMessage()
	{
		if (_primarySending) return;
		_primarySending = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var submit = new CreateTalkThread
			{
				VisibleInChannel = false,
				ChannelId = _channel.Id,
				Content = _primaryContent,
				ThreadId = null,
				Type = TalkThreadType.Comment,
				UserId = TfUserState.Value.CurrentUser.Id,
				RowIds = new List<Guid> { _rowId }
			};
			var result = TalkService.CreateThread(submit);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message is sent"));
				_primaryContent = null;
				var getThreadsResult = TalkService.GetThreads(_channel.Id, _skValue);
				if (getThreadsResult.IsSuccess) _threads = getThreadsResult.Value;
				else throw new Exception("GetThreads failed");
				_generateThreadClassDict();
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_primarySending = false;
			await InvokeAsync(StateHasChanged);
		}
	}

	private void _generateThreadClassDict()
	{
		_threadClassDict.Clear();
		int mainIndex = 0;
		var threadsReversed = _threads.OrderBy(x => x.CreatedOn).ToList();
		foreach (var message in threadsReversed)
		{
			var isFirst = mainIndex == 0;
			var isLast = mainIndex == threadsReversed.Count - 1;
			var prevMain = isFirst ? null : threadsReversed[mainIndex - 1];
			var nextMain = isLast ? null : threadsReversed[mainIndex + 1];
			var cssList = new List<string>();
			if (message.Content == "<p>saSas</p>")
			{
				var boz = 0;
				var boz2 = (message.CreatedOn - prevMain.CreatedOn).TotalMinutes;
			}
			if (message.Content == "<p>dedweddeded</p>")
			{
				var boz = 0;
				var boz2 = (message.CreatedOn - prevMain.CreatedOn).TotalMinutes;
			}
			if (prevMain is not null
				&& prevMain.User.Id == message.User.Id
				&& (message.CreatedOn - prevMain.CreatedOn).TotalMinutes <= 5)
			{
				cssList.Add("talk-followup");
			}

			_threadClassDict[message.Id] = String.Join(" ", cssList);

			int subIndex = 0;
			foreach (var subMessage in message.SubThread)
			{
				if (subMessage.Id == message.Id) continue;
				var isSubFirst = subIndex == 1; //first should be the master
				var isSubLast = subIndex == message.SubThread.Count - 1;
				var prevSub = isSubFirst ? null : message.SubThread[mainIndex - 1];
				var nextSub = isSubLast ? null : message.SubThread[mainIndex + 1];
				var cssSubList = new List<string>();
				_threadClassDict[subMessage.Id] = String.Join(" ", cssSubList);

				subIndex++;
			}

			mainIndex++;
		}
	}

	private async Task _replyToThread(TalkThread thread)
	{
		_activeThreadId = _activeThreadId != thread.Id ? thread.Id : null;
	}

	private async Task _editThread(TalkThread thread)
	{
		if (_threadEditedId is not null)
		{
			if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("You will loose any unsaved changes on your previous edit. Do you want to continue?")))
				return;
		}

		if (_threadEditedId == thread.Id) _threadEditedId = null;
		else _threadEditedId = thread.Id;
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

	private async Task _saveMessage(TalkThread thread)
	{
		if (_threadIdUpdateSaving is not null) return;
		_threadIdUpdateSaving = thread.Id;
		await InvokeAsync(StateHasChanged);
		try
		{
			var result = TalkService.UpdateThread(thread.Id, thread.Content);
			ProcessServiceResponse(result);
			if (result.IsSuccess)
			{
				ToastService.ShowSuccess(LOC("Message saved"));
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_threadIdUpdateSaving = null;
			_threadEditedId = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private Task _cancelSaveMessage()
	{

		_threadEditedId = null;
		return Task.CompletedTask;
	}
}

public record TalkThreadPanelContext
{
	public Guid? ChannelId { get; set; }
	public TfDataTable DataTable { get; set; } = null;
	public int RowIndex { get; set; } = -1;

}
