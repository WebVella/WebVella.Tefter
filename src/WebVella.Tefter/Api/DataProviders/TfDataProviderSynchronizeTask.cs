namespace WebVella.Tefter;

public class TfDataProviderSynchronizeTask
{
	public Guid Id { get; set; }
	public Guid DataProviderId { get; set; }
	public TfSynchronizationPolicy Policy { get; set; }
	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;
	public DateTime CreatedOn { get; set; } = DateTime.Now;
	public DateTime? StartedOn { get; set; } = null;
	public DateTime? CompletedOn { get; set; } = null;
}


[DboModel("data_provider_synchronize_task")]
public class TfDataProviderSynchronizeTaskDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("data_provider_id")]
	public Guid DataProviderId { get; set; }

	[DboModelProperty("policy_json")]
	public string PolicyJson { get; set; }

	[DboModelProperty("status")]
	[DboTypeConverter(typeof(EnumPropertyConverter<TfSynchronizationStatus>))]
	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; } = DateTime.Now;

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("started_on")]
	public DateTime? StartedOn { get; set; } = null;

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("completed_on")]
	public DateTime? CompletedOn { get; set; } = null;
}


[DboModel("data_provider_synchronize_result_info")]
public class TfDataProviderSynchronizeResultInfoDbo
{
	[DboModelProperty("id")]
	public Guid Id { get; set; }

	[DboModelProperty("task_id")]
	public Guid TaskId { get; set; }

	[DboTypeConverter(typeof(DateTimePropertyConverter))]
	[DboModelProperty("created_on")]
	public DateTime CreatedOn { get; set; } = DateTime.Now;

	[DboModelProperty("tf_row_index")]
	public int? TfRowIndex { get; set; } = null;

	[DboModelProperty("tf_id")]
	public Guid? TfId { get; set; } = null;


	[DboModelProperty("info")]
	public string Info { get; set; } = null;

	[DboModelProperty("warning")]
	public string Warning { get; set; } = null;

	[DboModelProperty("error")]
	public string Error { get; set; } = null;
}