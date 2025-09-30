

using WebVella.Tefter.UI.Components;
using WebVella.Tefter.UIServices;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkThreadComponent : TfBaseComponent, IDisposable
{
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkChannel? Channel { get; set; } = null;
	[Parameter] public string? DataIdentityValue { get; set; } = null;
	[Parameter] public TfSpacePageAddonContext? Context { get; set; } = null;
	[Parameter] public string Style { get; set; } = "";
	[Parameter] public RenderFragment HeaderActions { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; } = default!;

	private string _error = string.Empty;
	private bool _isLoading = true;

	private TucEditor _channelEditor = null;
	private string _channelEditorContent = null;
	private bool _channelEditorSending = false;

	private TucEditor _threadEditor = null;
	private string _threadEditorContent = null;
	private bool _threadEditorSending = false;

	private TalkThread _activeThread = null;

	private Guid _rowId = Guid.Empty;
	private List<TalkThread> _threads = new();

	private Guid? _threadEditedId = null;
	private Guid? _subthreadEditedId = null;
	private Guid? _threadIdUpdateSaving = null;
	private bool _threadVisibleInChannel = false;

	private string _dataIdentityValue = null;
	private Guid? _channelId = null;
	private Guid? _pageId = null;
	private FluentSearch? _refSearch = null;

	public void Dispose()
	{
		TalkService.ThreadCreated -= On_ThreadChanged;
		TalkService.ThreadUpdated -= On_ThreadChanged;
		TalkService.ThreadDeleted -= On_ThreadChanged;
		TfUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await _init();
		_isLoading = false;
		TalkService.ThreadCreated += On_ThreadChanged;
		TalkService.ThreadUpdated += On_ThreadChanged;
		TalkService.ThreadDeleted += On_ThreadChanged;
		TfUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender && _refSearch != null)
		{
			if (Context.SpacePage?.Id != _pageId)
			{
				_refSearch.FocusAsync();
				_pageId = Context.SpacePage.Id;
			}
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		if (_channelId == Channel?.Id && _dataIdentityValue == DataIdentityValue)
			return;
		await _init();
		await InvokeAsync(StateHasChanged);
	}

	private async void On_ThreadChanged(object? caller, TalkThread args)
	{
		await _init();
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(navState: args);
	}
	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState is null)
			navState = TfAuthLayout.NavigationState;
		try
		{
			_threads = new();
			if (Channel is null) return;
			_threads = TalkService.GetThreads(
				channelId: Channel.Id,
				dataIdentityValue: DataIdentityValue);
			_dataIdentityValue = DataIdentityValue;
			_channelId = Channel.Id;
			if (_pageId is not null && Context.SpacePage?.Id != _pageId)
			{
				_refSearch?.FocusAsync();
				_pageId = Context.SpacePage?.Id;
			}

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
			await Task.Delay(100);
			if (_channelEditor is not null)
				await _channelEditor.Focus();
		}
		finally
		{
			_isLoading = false;
			UriInitialized = navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _sendMessage()
	{
		if (_channelEditorSending) return;
		_channelEditorSending = true;
		await InvokeAsync(StateHasChanged);
		try
		{
			var submit = new CreateTalkThreadWithDataIdentityModel
			{
				ChannelId = Channel.Id,
				Content = _channelEditorContent,
				Type = TalkThreadType.Comment,
				UserId = Context.CurrentUser.Id,
				DataIdentityValues = new List<string>() { _dataIdentityValue }
			};
			var thread = TalkService.CreateThread(submit);
			ToastService.ShowSuccess(LOC("Message is sent"));
			_channelEditorContent = null;
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
				UserId = Context.CurrentUser.Id,
			};
			var thread = TalkService.CreateSubThread(submit);
			ToastService.ShowSuccess(LOC("Message is sent"));
			_threadEditorContent = null;
			_activeThread = TalkService.GetThread(_activeThread.Id);
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
				&& (message.ConnectedDataIdentityValuesCount <= 1)
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
			TalkService.DeleteThread(thread.Id);
			ToastService.ShowSuccess(LOC("Message deleted"));
			int threadsIndex = _threads.FindIndex(x => x.Id == thread.Id);
			int subthreadsIndex = _activeThread is null ? -1 : _activeThread.SubThread.FindIndex(x => x.Id == thread.Id);
			var now = DateTime.Now;
			if (threadsIndex > -1) _threads[threadsIndex].DeletedOn = now;
			if (subthreadsIndex > -1) _activeThread.SubThread[subthreadsIndex].DeletedOn = now;
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
			TalkService.UpdateThread(thread.Id, content);
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
}
