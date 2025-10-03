using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;
using WebVella.Tefter.Exceptions;
using WebVella.Tefter.Models;
using WebVella.Tefter.UI.Components;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.DataProviders.Csv.Addons;

public partial class ManageSettingsComponent : TfFormBaseComponent,
    ITfScreenRegionComponent<TfDataProviderManageSettingsScreenRegionContext>
{
    public const string ID = "8edf466e-74d1-42f0-b166-8df2c4e3e1b9";
    public const string NAME = "CSV Data Provider Manage Settings";
    public const string DESCRIPTION = "";
    public const string FLUENT_ICON_NAME = "PuzzlePiece";
    public const int POSITION_RANK = 1000;

    public Guid AddonId { get; init; } = new Guid(ID);
    public string AddonName { get; init; } = NAME;
    public string AddonDescription { get; init; } = DESCRIPTION;
    public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
    public int PositionRank { get; init; } = POSITION_RANK;
    public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){
        new TfScreenRegionScope(typeof(CsvDataProvider),null)
    };
    [Parameter] public TfDataProviderManageSettingsScreenRegionContext RegionContext { get; set; }

    private string _advancedSettings
    {
        get
        {
            if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new CsvDataProviderSettingsAdvanced());
            return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    private CsvDataProviderSettings _form = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(RegionContext.SettingsJson);
        RegionContext.SetValidate(_validate);
        base.InitForm(_form);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
        {
            _form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(RegionContext.SettingsJson);
            base.InitForm(_form);
        }
    }

    private List<ValidationError> _validate()
    {
        MessageStore.Clear();
        var errors = new List<ValidationError>();

        if (String.IsNullOrWhiteSpace(_form.Filepath))
        {
            errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("required")));
        }
        else
        {
            string extension = Path.GetExtension(_form.Filepath);
            if (extension != ".csv")
            {
                errors.Add(new ValidationError(nameof(CsvDataProviderSettings.Filepath), LOC("'csv' file extension is required")));
            }
        }

        if (!String.IsNullOrWhiteSpace(_form.CultureName))
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            var culture = cultures.FirstOrDefault(c => c.Name.Equals(_form.CultureName, StringComparison.OrdinalIgnoreCase));
            if (culture == null)
                errors.Add(new ValidationError(nameof(CsvDataProviderSettings.CultureName), LOC("invalid. format like 'en-US'")));
        }

        foreach (var item in errors)
        {
            MessageStore.Add(EditContext.Field(item.PropertyName), item.Message);
        }
        EditContext.Validate();
        StateHasChanged();
        EditContext.Validate();
        return errors;
    }

    private void _getCultureFromServer()
    {
        _form.CultureName = Thread.CurrentThread.CurrentCulture.Name;
    }
    private async Task _changeAdvancedSettings(string value)
    {
        try
        {
            _form.AdvancedSetting = JsonSerializer.Deserialize<CsvDataProviderSettingsAdvanced>(value);
            await _valueChanged();
        }
        catch (Exception ex)
        {
            ToastService.ShowError(ex.Message);
        }
    }

    private async Task _valueChanged()
    {
        RegionContext.SettingsJson = JsonSerializer.Serialize(_form);
        await RegionContext.SettingsJsonChanged.InvokeAsync(RegionContext.SettingsJson);
    }

    private async Task _selectFile()
    {
        var dialog = await DialogService.ShowDialogAsync<TucFileRepositoryFileSelectDialog>(
                null,
                new DialogParameters()
                {
                    PreventDismissOnOverlayClick = true,
                    PreventScroll = true,
                    Width = TfConstants.DialogWidthLarge,
                    TrapFocus = false,
                });
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            _form.Filepath = (string?)result.Data;
            await _valueChanged();
            await InvokeAsync(StateHasChanged);
        }

    }
}