namespace WebVella.Tefter.DataProviders.Folder.Addons;

public partial class ManageSettingsComponent : TfFormBaseComponent,
    ITfScreenRegionComponent<TfDataProviderManageSettingsScreenRegionContext>
{
    public const string ID = "63b05937-6b5d-4409-9d76-665189ca2cb4";
    public const string NAME = "Folder Data Provider Manage Settings";
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
    [Parameter] public TfDataProviderManageSettingsScreenRegionContext RegionContext { get; set; }


    private FolderDataProviderSettings _form = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<FolderDataProviderSettings>(RegionContext.SettingsJson) ?? new();
        RegionContext.SetValidate(_validate);
        base.InitForm(_form);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
        {
            _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<FolderDataProviderSettings>(RegionContext.SettingsJson) ?? new();
            base.InitForm(_form);
        }
    }

    private List<ValidationError> _validate()
    {
        MessageStore.Clear();
        EditContext.Validate();
        StateHasChanged();
        EditContext.Validate();
        return new List<ValidationError>();
    }
}