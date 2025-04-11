using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.Csv.Addons;

[LocalizationResource("WebVella.Tefter.DataProviders.Csv.Addons.ScreenRegionComponents.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.DataProviders.Csv")]
public partial class ViewSettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("15fb7760-5ff5-425f-b41e-339d67282cc4");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "CSV Data Provider View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(CsvDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegionContext RegionContext { get; init; }

	private string _advancedSettings
	{
		get
		{
			if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new CsvDataProviderSettingsAdvanced());
			return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
		}
	}

	private CsvDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(RegionContext.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(RegionContext.SettingsJson);
		}
	}

}