using WebVella.Tefter.UI.Components;

namespace WebVella.Tefter.Talk.Components;
public partial class TalkThreadModal : TfFormBaseComponent, IDialogContentComponent<TalkThreadModalContext>
{
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkThreadModalContext Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = string.Empty;
	private string _error = string.Empty;
	private bool _isLoading = true;
	private bool _isSubmitting = false;
	private TucEditor _editor = null;
	private List<TalkChannel> _channels = new();
	private TalkChannel _selectedChannel = null;
	private string _content = null;


	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			if (Content is null
			|| Content.DataProviderId == Guid.Empty
			|| Content.SelectedRowIds is null || Content.SelectedRowIds.Count == 0) return;

			var providerIdentities = TalkService.GetDataProviderIdentities(Content.DataProviderId);
			var allChannels = TalkService.GetChannels();

			//Select only channels that are compatible with this DataProvider
			foreach (var channel in allChannels)
			{
				if (String.IsNullOrWhiteSpace(channel.DataIdentity)) continue;
				if (!providerIdentities.Any(x => x.DataIdentity == channel.DataIdentity)) continue;
				_channels.Add(channel);
			}

			if (_channels.Count == 1)
			{
				await _selectChannelHandler(_channels[0]);
			}
			else if (_channels.Count > 1)
			{
				_title = LOC("Select a channel");
			}


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
		await Task.Delay(1);
		try
		{
			var submit = new CreateTalkThreadWithRowIdModel
			{
				ChannelId = _selectedChannel.Id,
				Content = _content,
				Type = TalkThreadType.Comment,
				UserId = Content.CurrentUser.Id,
				RowIds = Content.SelectedRowIds,
				DataProviderId = Content.DataProviderId
			};
			TalkService.CreateThread(submit);
			ToastService.ShowSuccess(LOC("Message is sent"));
			_content = null;
			if (!String.IsNullOrWhiteSpace(_selectedChannel.DataIdentity)
				&& !String.IsNullOrWhiteSpace(_selectedChannel.CountSharedColumnName))
			{
				await TfEventBus.PublishAsync(
					key: TfAuthLayout.GetSessionId(),
					payload: new TfSpaceViewDataUpdatedEventPayload(Content.SpaceViewId,Content.SelectedRowIds));	 
			}

			await Dialog.CloseAsync();
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
	public TfUser CurrentUser { get; set; }
	public Guid DataProviderId { get; set; }
	public Guid SpaceViewId { get; set; }
	public List<Guid> SelectedRowIds { get; set; } = new();
	public Dictionary<Guid, Dictionary<string, long>> CountChange { get; set; } = new();

}