namespace WebVella.Tefter.DataProviders.Blank.Addons;

public partial class ViewSettingsAddon : TfBaseComponent,
    ITfScreenRegionAddon<TfDataProviderDisplaySettingsScreenRegion>
{
    public const string ID = "8bd9a47b-4a84-408e-a07e-cd8af1e2e2e0";
    public const string NAME = "Blank Data Provider View Settings";
    public const string DESCRIPTION = "";
    public const string FLUENT_ICON_NAME = "PuzzlePiece";
    public const int POSITION_RANK = 1000;
    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
    public int PositionRank { get; init; } = POSITION_RANK;
    public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
        new TfScreenRegionScope(typeof(BlankDataProvider),null)
    };
    [Parameter] public TfDataProviderDisplaySettingsScreenRegion RegionContext { get; set; }

    private BlankDataProviderSettings _form = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<BlankDataProviderSettings>(RegionContext.SettingsJson);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
        {
            _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<BlankDataProviderSettings>(RegionContext.SettingsJson);
        }
    }

}