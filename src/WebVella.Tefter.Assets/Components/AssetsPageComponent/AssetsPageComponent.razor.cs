using WebVella.Tefter.MessagingEx;

namespace WebVella.Tefter.Assets.Components;

public partial class AssetsPageComponent : TfBaseComponent, IAsyncDisposable
{
    [Inject] protected IAssetsService AssetsService { get; set; } = null!;
    [Inject] protected ITfEventBusEx TfEventBus { get; set; } = null!;  
    [Parameter] public Guid SpacePageId { get; set; }

    private AssetsSpacePageComponentOptions _options = new();
    private List<AssetsFolder> _folders = new();
    private AssetsFolder? _folderSelected;    
    private IAsyncDisposable _spacePageUpdatedEventSubscriber = null!;	
    public async ValueTask DisposeAsync()
    {
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
                handler:On_SpacePageUpdatedEventAsync);
    }

    private Task On_SpacePageUpdatedEventAsync(string? key, TfSpacePageUpdatedEventPayload? payload)
    {
        _init();
        return Task.CompletedTask;
    }

    private void _init()
    {
        var spacePage = TfService.GetSpacePage(SpacePageId);
        _options = new AssetsSpacePageComponentOptions();
        if (!String.IsNullOrWhiteSpace(spacePage.ComponentOptionsJson))
            _options = JsonSerializer.Deserialize<AssetsSpacePageComponentOptions>(spacePage.ComponentOptionsJson) ??
                       new AssetsSpacePageComponentOptions();
        _folders = AssetsService.GetFolders();
        
        _folderSelected = _folders.FirstOrDefault(x => x.Id == _options.FolderId);
    }

    private async Task _editFolder()
    {
        var dialog = await DialogService.ShowDialogAsync<AssetsPageManageDialog>(
            new AssetsPageManageDialogContext(){ Options = _options,Folders = _folders, SpacePageId = SpacePageId},
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