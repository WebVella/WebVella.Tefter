using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.MsSql.Components;

[LocalizationResource("WebVella.Tefter.DataProviders.MsSql.Components.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.DataProviders.MsSql")]
public partial class ViewSettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfDataProviderDisplaySettingsComponentContext>
{
	public Guid Id { get; init; } = new Guid("26621b7b-c7de-4a27-8330-ee33cf0a807f");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "MSSQL Data Provider View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfRegionComponentScope> Scopes { get; init; } = new List<TfRegionComponentScope>(){ 
		new TfRegionComponentScope(typeof(MsSqlDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsComponentContext Context { get; init; }

	private MsSqlDataProviderSettings _form = new();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Context.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<MsSqlDataProviderSettings>(Context.SettingsJson);
		}
	}
}