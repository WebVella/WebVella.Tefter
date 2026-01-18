namespace WebVella.Tefter.Talk.Components;

public partial class TalkGlobalState : TfBaseComponent,
    ITfScreenRegionAddon<TfGlobalStateScreenRegion>, IAsyncDisposable
{
    public const string ID = "9C5500A7-F0C2-4151-AEC7-2C08A50258EF";
    public const string NAME = "Talk Global State";
    public const string DESCRIPTION = "";
    public const string FLUENT_ICON_NAME = "CommentMultiple";
    public const int POSITION_RANK = 100;
    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
    public int PositionRank { get; init; } = POSITION_RANK;
    public List<TfScreenRegionScope> Scopes { get; init; } = new();

    [Inject] public ITalkService TalkService { get; set; }
    [Parameter] public TfGlobalStateScreenRegion RegionContext { get; set; }

    private IAsyncDisposable? _addonEventSubscriber = null;

    public async ValueTask DisposeAsync()
    {
        if (_addonEventSubscriber is not null)
            await _addonEventSubscriber.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        _addonEventSubscriber = await TfEventBus.SubscribeAsync<TfAddonEventPayload>(
            handler: On_AddonEvent,
            matchKey: (key) => key == TfAuthLayout.GetSessionId().ToString());
    }

    private async Task On_AddonEvent(string? key, TfAddonEventPayload? payload)
    {
        if (payload is null) return;
        if (payload.AddonId != new Guid(TalkApp.ID)) return;
        switch (payload.EventName)
        {
            case "ITalkService-CreateThread-WithRowIdModel":
                await On_ITalkService_CreateThread(payload);
                break;
        }
    }

    private Task On_ITalkService_CreateThread(TfAddonEventPayload payload)
    {
        if(String.IsNullOrWhiteSpace(payload.EventJson)) return Task.CompletedTask;
        try
        {
            var createThreadObj =
                JsonSerializer.Deserialize<CreateTalkThreadWithRowIdModel>(payload.EventJson);
            if(createThreadObj is null) return Task.CompletedTask;
            TalkService.CreateThread(createThreadObj);
            //TODO BOZ: Add event publishing when Space View ONData Change is event driven
        }
        catch (Exception)
        {
            //Ignore
        }

        return Task.CompletedTask;
    }
}