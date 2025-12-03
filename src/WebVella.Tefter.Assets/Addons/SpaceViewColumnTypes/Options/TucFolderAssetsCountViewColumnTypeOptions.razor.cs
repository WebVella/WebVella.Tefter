using WebVella.Tefter.Exceptions;

namespace WebVella.Tefter.UI.Addons;

public partial class TucFolderAssetsCountViewColumnTypeOptions : TfLocalizedViewColumnComponent
{
    [Parameter] public TfSpaceViewColumnOptionsMode Context { get; set; } = null!;
    [Parameter] public EventCallback<TfFolderAssetsCountViewColumnTypeSettings> SettingsChanged { get; set; }
    [Parameter] public EventCallback<Tuple<string, string?>> DataMappingChanged { get; set; }
    [Parameter] public List<AssetsFolder> FolderOptions { get; set; } = new();


    private TfFolderAssetsCountViewColumnTypeSettings _settings =  new ();

    protected override void OnParametersSet()
    {
        _settings = Context.GetSettings<TfFolderAssetsCountViewColumnTypeSettings>();
    }       
    
    private async Task _folderSelectHandler(AssetsFolder? folder)
    {
        _settings.FolderId = folder?.Id;
        await SettingsChanged.InvokeAsync(_settings);
        if (folder is not null)
            await DataMappingChanged.InvokeAsync(new Tuple<string, string?>("Value",
                $"{folder.DataIdentity}.{folder.CountSharedColumnName}"));
        else
            await DataMappingChanged.InvokeAsync(new Tuple<string, string?>("Value", null));
    }
}