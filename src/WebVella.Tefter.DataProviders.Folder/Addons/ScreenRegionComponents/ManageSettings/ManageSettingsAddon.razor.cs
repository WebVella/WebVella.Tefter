namespace WebVella.Tefter.DataProviders.Folder.Addons;

public partial class ManageSettingsAddon : TfFormBaseComponent,
    ITfScreenRegionAddon<TfDataProviderManageSettingsScreenRegion>
{
    public const string ID = "04caf444-0566-4256-b0e4-151429dd9d60";
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
    [Parameter] public TfDataProviderManageSettingsScreenRegion RegionContext { get; set; }


    private FolderDataProviderSettings _form = new();
    private string? _error = null;

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
            if (_form.Shares.Count == 0)
                _addShare();
            base.InitForm(_form);
        }
    }

    private List<ValidationError> _validate()
    {
        MessageStore.Clear();
        EditContext.Validate();

        _error = null;
        foreach (var share in _form.Shares)
        {
            if(String.IsNullOrWhiteSpace(share.Path))
                _error = "One or more paths are empty";
        }
        if(_error is not null)
            MessageStore.Add(EditContext.Field(nameof(_form.Shares)), _error);

        StateHasChanged();
        EditContext.Validate();
        return new List<ValidationError>();
    }

    private async Task _valueChanged()
    {
        RegionContext.SettingsJson = JsonSerializer.Serialize(_form);
        await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.SettingsJson);
    }

    private void _addShare()
    {
        _form.Shares.Add(new FolderDataProviderShareSettings());
    }
    private void _removeShare(FolderDataProviderShareSettings share)
    {
        _form.Shares.Remove(share);
    }
}
