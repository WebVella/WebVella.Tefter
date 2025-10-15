using Microsoft.AspNetCore.Components.Routing;

namespace WebVella.Tefter.Talk.Components;

public partial class TalkChannelAdminList : TfBaseComponent
{
    [Inject] public ITalkService TalkService { get; set; }

    private List<TalkChannel> _channels = new();
    private TfNavigationState _navState = new();

    public void Dispose()
    {
        TalkService.ChannelCreated -= On_ChannelChanged;
        TalkService.ChannelUpdated -= On_ChannelChanged;
        TalkService.ChannelDeleted -= On_ChannelChanged;
        Navigator.LocationChanged -= On_NavigationStateChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await _init(TfAuthLayout.GetState().NavigationState);
        TalkService.ChannelCreated += On_ChannelChanged;
        TalkService.ChannelUpdated += On_ChannelChanged;
        TalkService.ChannelDeleted += On_ChannelChanged;
        Navigator.LocationChanged += On_NavigationStateChanged;
    }

    private async Task On_ChannelChanged(TalkChannel args)
    {
        await InvokeAsync(async () => { await _init(TfAuthLayout.GetState().NavigationState); });
    }

    private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
    {
        InvokeAsync(async () =>
        {
            if (UriInitialized != args.Location)
                await _init(TfAuthLayout.GetState().NavigationState);
        });
    }

    private async Task _init(TfNavigationState navState)
    {
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
            new ()
            {
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
                Width = TfConstants.DialogWidthLarge,
                TrapFocus = false
            });
        var result = await dialog.Result;
        if (!result.Cancelled && result.Data != null)
        {
        }
    }

    private async Task _editChannelHandler(TalkChannel channel)
    {
        var dialog = await DialogService.ShowDialogAsync<TalkChannelManageDialog>(
            channel,
            new ()
            {
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
                Width = TfConstants.DialogWidthLarge,
                TrapFocus = false
            });
        var result = await dialog.Result;
        if (!result.Cancelled && result.Data != null)
        {
        }
    }

    private async Task _deleteChannelHandler(TalkChannel channel)
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm",
                LOC(
                    "Are you sure that you need this channel deleted? This will delete all threads and comments in it!")))
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