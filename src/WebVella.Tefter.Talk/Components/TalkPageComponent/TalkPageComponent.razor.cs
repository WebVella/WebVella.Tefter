namespace WebVella.Tefter.Talk.Components;

public partial class TalkPageComponent : TfBaseComponent, IAsyncDisposable
{
    [Inject] protected ITalkService TalkService { get; set; } = null!;
    [Parameter] public Guid SpacePageId { get; set; }
    private TalkSpacePageComponentOptions _options = new();
    private List<TalkChannel> _channels = new();
    private TalkChannel? _channelSelected;
    private IAsyncDisposable? _spacePageUpdatedEventSubscriber = null;

    public async ValueTask DisposeAsync()
    {
        if (_spacePageUpdatedEventSubscriber is not null)
            await _spacePageUpdatedEventSubscriber.DisposeAsync();
    }

    protected override void OnInitialized()
    {
        _init();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            _spacePageUpdatedEventSubscriber = await TfEventBus.SubscribeAsync<TfSpacePageUpdatedEventPayload>(
                handler: On_SpacePageUpdatedEventAsync,
                matchKey: (key) => key == TfAuthLayout.GetSessionId().ToString());
    }

    private Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
    {
        _init();
        return Task.CompletedTask;
    }

    private void _init()
    {
        var spacePage = TfService.GetSpacePage(SpacePageId);
        if (spacePage is null) return;
        _options = new TalkSpacePageComponentOptions();
        if (!String.IsNullOrWhiteSpace(spacePage.ComponentOptionsJson))
            _options = JsonSerializer.Deserialize<TalkSpacePageComponentOptions>(spacePage.ComponentOptionsJson) ??
                       new TalkSpacePageComponentOptions();
        _channels = TalkService.GetChannels();

        _channelSelected = _channels.FirstOrDefault(x => x.Id == _options.ChannelId);
    }

    private async Task _editFolder()
    {
        var dialog = await DialogService.ShowDialogAsync<TalkPageManageDialog>(
            new TalkPageManageDialogContext() { Options = _options, Channels = _channels, SpacePageId = SpacePageId },
            new()
            {
                PreventDismissOnOverlayClick = true,
                PreventScroll = true,
                Width = TfConstants.DialogWidthLarge,
                TrapFocus = false
            });
        _ = await dialog.Result;
    }
}