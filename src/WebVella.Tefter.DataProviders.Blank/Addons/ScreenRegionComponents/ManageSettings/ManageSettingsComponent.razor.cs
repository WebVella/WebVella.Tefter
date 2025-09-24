using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.UI.Components;
using WebVella.Tefter.UIServices;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Blank.Addons;

public partial class ManageSettingsComponent : TfFormBaseComponent,
    ITfScreenRegionComponent<TfDataProviderManageSettingsScreenRegionContext>
{
    public const string ID = "3aa866d7-a639-494f-9acc-104c89a84a80";
    public const string NAME = "Blank Data Provider Manage Settings";
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
    [Parameter] public TfDataProviderManageSettingsScreenRegionContext RegionContext { get; set; }

  
    private BlankDataProviderSettings _form = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<BlankDataProviderSettings>(RegionContext.SettingsJson);
        RegionContext.SetValidate(_validate);
        base.InitForm(_form);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
        {
            _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<BlankDataProviderSettings>(RegionContext.SettingsJson);
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