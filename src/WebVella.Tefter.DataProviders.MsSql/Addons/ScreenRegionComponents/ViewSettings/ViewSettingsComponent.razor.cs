using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Addons;

[LocalizationResource("WebVella.Tefter.DataProviders.MsSql.Addons.ScreenRegionComponents.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.DataProviders.MsSql")]
public partial class ViewSettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>
{
	public Guid Id { get; init; } = new Guid("26621b7b-c7de-4a27-8330-ee33cf0a807f");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "MSSQL Data Provider View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegionContext RegionContext { get; init; }

	private MsSqlDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(RegionContext.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (RegionContext.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(RegionContext.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(RegionContext.SettingsJson);
		}
	}
}