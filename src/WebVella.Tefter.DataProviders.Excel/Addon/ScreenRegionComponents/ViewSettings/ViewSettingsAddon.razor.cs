using WebVella.Tefter.Models;

namespace WebVella.Tefter.DataProviders.Excel.Addons;

public partial class ViewSettingsAddon : TfBaseComponent,
	ITfScreenRegionAddon<TfDataProviderDisplaySettingsScreenRegion>
{
	public const string ID = "e44b634d-6c82-4cb5-8bc4-8048985f6c5e";
	public const string NAME = "Excel Data Provider View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(ExcelDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegion RegionContext { get; set; }
    private string _advancedSettings
    {
        get
        {
            if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new ExcelDataProviderSettingsAdvanced());
            return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    private ExcelDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<ExcelDataProviderSettings>(RegionContext.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<ExcelDataProviderSettings>(RegionContext.SettingsJson);
		}
	}

}