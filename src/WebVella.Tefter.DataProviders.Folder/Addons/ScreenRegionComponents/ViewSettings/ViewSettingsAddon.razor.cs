namespace WebVella.Tefter.DataProviders.Folder.Addons;

public partial class ViewSettingsAddon : TfBaseComponent,
    ITfScreenRegionAddon<TfDataProviderDisplaySettingsScreenRegion>
{
    public const string ID = "0518a537-cdef-49e4-ad12-0c456cedb267";
    public const string NAME = "Folder Data Provider View Settings";
    public const string DESCRIPTION = "";
    public const string FLUENT_ICON_NAME = "PuzzlePiece";
    public const int POSITION_RANK = 1000;
    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
    public int PositionRank { get; init; } = POSITION_RANK;
    public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
        new TfScreenRegionScope(typeof(FolderDataProvider),null)
    };
    [Parameter] public TfDataProviderDisplaySettingsScreenRegion RegionContext { get; set; }

    private FolderDataProviderSettings _form = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<FolderDataProviderSettings>(RegionContext.SettingsJson) ?? new();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
        {
            _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<FolderDataProviderSettings>(RegionContext.SettingsJson) ?? new();
        }
    }

}