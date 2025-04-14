namespace WebVella.Tefter.Models;

//[TfDboModel("tf_data_provider_synchronize_task")]
//public class TfDataProviderSynchronizeTaskExtended
//{
//	[TfDboModelProperty("id")]
//	public Guid Id { get; set; }

//	[TfDboModelProperty("data_provider_id")]
//	public Guid DataProviderId { get; set; }

//	[TfDboModelProperty("policy_json")]
//	public string PolicyJson { get; set; }

//	[TfDboModelProperty("status")]
//	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfSynchronizationStatus>))]
//	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;

//	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
//	[TfDboModelProperty("created_on")]
//	public DateTime CreatedOn { get; set; } = DateTime.Now;

//	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
//	[TfDboModelProperty("started_on")]
//	public DateTime? StartedOn { get; set; } = null;

//	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
//	[TfDboModelProperty("completed_on")]
//	public DateTime? CompletedOn { get; set; } = null;

//	[TfDboModelProperty("info_count")]
//	public long InfoCount { get; set; } = 0;

//	[TfDboModelProperty("warning_count")]
//	public long WarningCount { get; set; } = 0;

//	[TfDboModelProperty("error_count")]
//	public long ErrorCount { get; set; } = 0;
//}

public class TfDataProviderSynchronizeTask
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	//temp initialized here, later will be in provider
	public TfSynchronizationPolicy Policy { get; set; } = new TfSynchronizationPolicy();
	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;
	public DateTime CreatedOn { get; set; } = DateTime.Now;
	public DateTime? StartedOn { get; set; } = null;
	public DateTime? CompletedOn { get; set; } = null;
	internal TfDataProviderSychronizationLog SynchronizationLog { get; set; } = new TfDataProviderSychronizationLog();
}

[TfDboModel("tf_data_provider_synchronize_task")]
public class TfDataProviderSynchronizeTaskDbo
{
	[TfDboModelProperty("id")]
	public Guid Id { get; set; }

	[TfDboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[TfDboModelProperty("policy_json")]
	public string PolicyJson { get; set; }

	[TfDboModelProperty("status")]
	[TfDboTypeConverter(typeof(TfEnumPropertyConverter<TfSynchronizationStatus>))]
	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; } = DateTime.Now;

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("started_on")]
	public DateTime? StartedOn { get; set; } = null;

	[TfDboTypeConverter(typeof(TfDateTimePropertyConverter))]
	[TfDboModelProperty("completed_on")]
	public DateTime? CompletedOn { get; set; } = null;

	[TfDboModelProperty("synch_log_json")]
	public string SynchLogJson { get; set; } = "[]";
}