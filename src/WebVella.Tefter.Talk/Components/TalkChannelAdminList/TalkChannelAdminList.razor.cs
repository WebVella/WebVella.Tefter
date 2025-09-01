using Microsoft.JSInterop;
using WebVella.Tefter.UIServices;

namespace WebVella.Tefter.Talk.Components;
public partial class TalkChannelAdminList : TfBaseComponent
{
	[Inject] public ITalkService TalkService { get; set; }
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private List<TalkChannel> _channels = new();
	private TfNavigationState _navState = new();

	public void Dispose()
	{
		TalkService.ChannelCreated -= On_ChannelChanged;
		TalkService.ChannelUpdated -= On_ChannelChanged;
		TalkService.ChannelDeleted -= On_ChannelChanged;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		TalkService.ChannelCreated += On_ChannelChanged;
		TalkService.ChannelUpdated += On_ChannelChanged;
		TalkService.ChannelDeleted += On_ChannelChanged;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
	private async void On_ChannelChanged(object? caller, TalkChannel args)
	{
		await _init();
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			_channels = TalkService.GetChannels();
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}


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
			ToastService.ShowSuccess(LOC("Channel successfully updated!"));
		}
	}
	private async Task _deleteChannelHandler(TalkChannel channel)
	{
		if (!await JSRuntime.InvokeAsync<bool>("confirm", LOC("Are you sure that you need this channel deleted? This will delete all threads and comments in it!")))
			return;
		try
		{
			TalkService.DeleteChannel(channel.Id);
			ToastService.ShowSuccess(LOC("Channel successfully deleted!"));
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