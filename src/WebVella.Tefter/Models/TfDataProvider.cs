using System.ComponentModel.DataAnnotations;

namespace WebVella.Tefter.Models;

public class TfDataProvider
{
	public Guid Id { get; internal set; }
	[Required]
	public string Name { get; internal set; }
	public int Index { get; internal set; }
	public string SettingsJson { get; internal set; }
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;
	public ReadOnlyCollection<TfDataProviderSystemColumn> SystemColumns { get; internal set; }
	public ReadOnlyCollection<TfDataProviderColumn> Columns { get; internal set; }
	public ReadOnlyCollection<TfSharedColumn> SharedColumns { get; internal set; }
	public ReadOnlyCollection<TfDataProviderJoinKey> JoinKeys { get; internal set; }
	public ReadOnlyCollection<string> SynchPrimaryKeyColumns { get; internal set; }
	public ITfDataProviderAddon ProviderType { get; internal set; }
	public IServiceProvider ServiceProvider { get; internal set; }
	public ReadOnlyCollection<string> SupportedSourceDataTypes => ProviderType.GetSupportedSourceDataTypes();
	public ReadOnlyCollection<TfDataProviderDataRow> GetRows(ITfDataProviderSychronizationLog log)
	{
		return ProviderType.GetRows(this,log);
	}
}

public record TfDataProviderModel
{
	[Required]
	public Guid Id { get; internal set; }
	[Required]
	public string Name { get; internal set; }
	[Required]
	public ITfDataProviderAddon ProviderType { get; internal set; }
	public string SettingsJson { get; internal set; } = null;
	public List<string> SynchPrimaryKeyColumns { get; set; } = new();
	[Required]
	public short SynchScheduleMinutes { get; set; } = 60;
	public bool SynchScheduleEnabled { get; set; } = false;
}


[DboCacheModel]
[TfDboModel("tf_data_provider")]
internal record TfDataProviderDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("name")]
	public string Name { get; set; }

	[TfDboAutoIncrementModel]
	[TfDboModelProperty("index")]
	public int Index { get; set; }

	[TfDboModelProperty("type_id")]
	public Guid TypeId { get; set; }

	[TfDboModelProperty("settings_json")]
	public string SettingsJson { get; set; } = null;

	[TfDboModelProperty("sync_primary_key_columns_json")]
	public string SynchPrimaryKeyColumnsJson { get; set; } = "[]";

	[TfDboModelProperty("synch_schedule_minutes")]
	public short SynchScheduleMinutes { get; set; } = 60;

	[TfDboModelProperty("synch_schedule_enabled")]
	public bool SynchScheduleEnabled { get; set; } = false;
}