namespace WebVella.Tefter.DataProviders.MsSql.Addons;

[LocalizationResource("WebVella.Tefter.DataProviders.MsSql.Addons.ScreenRegionComponents.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.DataProviders.MsSql")]
public partial class ViewSettingsComponent : TfBaseComponent,
	ITfScreenRegionComponent<TfDataProviderDisplaySettingsScreenRegionContext>
{
	public const string ID = "26621b7b-c7de-4a27-8330-ee33cf0a807f";
	public const string NAME = "MSSQL Data Provider View Settings";
	public const string DESCRIPTION = "";
	public const string FLUENT_ICON_NAME = "PuzzlePiece";
	public const int POSITION_RANK = 1000;
	public Guid AddonId { get; init; } = new Guid(ID);
	public string AddonName { get; init; } = NAME;
	public string AddonDescription { get; init; } = DESCRIPTION;
	public string AddonFluentIconName { get; init; } = FLUENT_ICON_NAME;
	public int PositionRank { get; init; } = POSITION_RANK;
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegionContext RegionContext { get; set; }

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