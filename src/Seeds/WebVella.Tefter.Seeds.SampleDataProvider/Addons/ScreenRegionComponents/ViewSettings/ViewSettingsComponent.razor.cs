using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.Seeds.SampleDataProvider.Components;

public partial class ViewSettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>
{
	public const string ID = "4872f1ca-778e-4076-9ca0-b2b8424b1093";
	public const string NAME = "Sample Data Provider View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid Id { get; init; } = new Guid(ID);
	public string Name { get; init; } = NAME;
	public string Description { get; init; } = DESCRIPTION;
	public string FluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(SampleDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegionContext RegionContext { get; init; }

	private string _advancedSettings
	{
		get
		{
			if (_form.AdvancedSetting is null) return JsonSerializer.Serialize(new SampleDataProviderSettingsAdvanced());
			return JsonSerializer.Serialize(_form.AdvancedSetting, new JsonSerializerOptions { WriteIndented = true });
		}
	}

	private SampleDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(RegionContext.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(RegionContext.SettingsJson);
		}
	}

}