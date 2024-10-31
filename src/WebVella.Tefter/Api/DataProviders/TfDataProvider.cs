using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter;

public class TfDataProvider
{
	public Guid Id { get; internal set; }
	[Required]
	public string Name { get; internal set; }
	public int Index { get; internal set; }
	public string SettingsJson { get; internal set; }
	public ReadOnlyCollection<TfDataProviderSystemColumn> SystemColumns { get; internal set; }
	public ReadOnlyCollection<TfDataProviderColumn> Columns { get; internal set; }
	public ReadOnlyCollection<TfSharedColumn> SharedColumns { get; internal set; }
	public ReadOnlyCollection<TfDataProviderSharedKey> SharedKeys { get; internal set; }
	public ITfDataProviderType ProviderType { get; internal set; }
	public IServiceProvider ServiceProvider { get; internal set; }
	public ReadOnlyCollection<string> SupportedSourceDataTypes => ProviderType.GetSupportedSourceDataTypes();
	public ReadOnlyCollection<TfDataProviderDataRow> GetRows()
	{
		return ProviderType.GetRows(this);
	}
}

public record TfDataProviderModel
{
	[Required]
	public Guid Id { get; internal set; }
	[Required]
	public string Name { get; internal set; }
	[Required]
	public ITfDataProviderType ProviderType { get; internal set; }

	public string SettingsJson { get; internal set; } = null;
}


[DboCacheModel]
[DboModel("data_provider")]
internal record TfDataProviderDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("name")]
	public string Name { get; set; }

	[DboAutoIncrementModel]
	[DboModelProperty("index")]
	public int Index { get; set; }

	[DboModelProperty("type_id")]
	public Guid TypeId { get; set; }

	[DboModelProperty("type_name")]
	public string TypeName { get; set; }

	[DboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = null;
}