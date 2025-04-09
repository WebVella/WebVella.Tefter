using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.Seeds.SampleDataProvider.Components;

public partial class ViewSettingsComponent : TfBaseComponent,
	ITfRegionComponent<TfDataProviderDisplaySettingsScreenRegion>
{
	public Guid Id { get; init; } = new Guid("4872f1ca-778e-4076-9ca0-b2b8424b1093");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "Sample Data Provider View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	public List<TfScreenRegionScope> Scopes { get; init; } = new List<TfScreenRegionScope>(){ 
		new TfScreenRegionScope(typeof(SampleDataProvider),null)
	};
	[Parameter] public TfDataProviderDisplaySettingsScreenRegion Context { get; init; }

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
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(Context.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<SampleDataProviderSettings>(Context.SettingsJson);
		}
	}

}