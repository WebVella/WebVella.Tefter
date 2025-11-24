namespace WebVella.Tefter.Talk.Components;

public partial class TalkPageComponent : TfBaseComponent, IAsyncDisposable
{
    [Inject] protected ITalkService TalkService { get; set; } = null!;
    [Inject] protected TfGlobalEventProvider TfEventProvider { get; set; } = null!;
    [Parameter] public Guid SpacePageId { get; set; }
    private TalkSpacePageComponentOptions _options = new();
    private List<TalkChannel> _channels = new();
    private TalkChannel? _channelSelected;

    public async ValueTask DisposeAsync()
    {
        await TfEventProvider.DisposeAsync();
    }

    protected override void OnInitialized()
    {
        _init();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            TfEventProvider.SpacePageUpdatedEvent += On_SpacePageChanged;
    }

    private Task On_SpacePageChanged(TfSpacePageUpdatedEvent args)
    {
        if (args.IsUserApplicable(this))
            _init();

        return Task.CompletedTask;
    }

    private void _init()
    {
        var spacePage = TfService.GetSpacePage(SpacePageId);
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