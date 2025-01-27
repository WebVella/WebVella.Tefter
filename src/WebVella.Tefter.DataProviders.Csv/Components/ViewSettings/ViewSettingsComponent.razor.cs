using WebVella.Tefter.Models;
using WebVella.Tefter.Web.Components;
using WebVella.Tefter.Web.Models;

namespace WebVella.Tefter.DataProviders.Csv.Components;

[LocalizationResource("WebVella.Tefter.DataProviders.Csv.Components.ViewSettings.ViewSettingsComponent", "WebVella.Tefter.DataProviders.Csv")]
public partial class ViewSettingsComponent : TfBaseComponent,
	ITfDynamicComponent<TfDataProviderDisplaySettingsComponentContext>,
	ITfComponentScope<CsvDataProvider>
{
	public Guid Id { get; init; } = new Guid("15fb7760-5ff5-425f-b41e-339d67282cc4");
	public int PositionRank { get; init; } = 1000;
	public string Name { get; init; } = "CSV Data Provider View Settings";
	public string Description { get; init; } = "";
	public string FluentIconName { get; init; } = "PuzzlePiece";
	[Parameter] public TfDataProviderDisplaySettingsComponentContext Context { get; init; }

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
		_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(Context.SettingsJson);
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		if (Context.SettingsJson != JsonSerializer.Serialize(_form))
		{
			_form = String.IsNullOrWhiteSpace(Context.SettingsJson) ? new() : JsonSerializer.Deserialize<CsvDataProviderSettings>(Context.SettingsJson);
		}
	}

}