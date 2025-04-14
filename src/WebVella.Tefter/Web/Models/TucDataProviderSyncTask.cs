namespace WebVella.Tefter.Web.Models;

public record TucDataProviderSyncTask
{
	public Guid Id { get; init; }
	public TucSynchronizationStatus Status { get; set; } = TucSynchronizationStatus.Pending;
	public DateTime CreatedOn { get; set; } = DateTime.Now;
	public string CreatedOnString
	{
		get
		{
			return $"{CreatedOn.ToShortDateString()} {CreatedOn.ToShortTimeString()}";
		}
	}
	public DateTime? StartedOn { get; set; } = null;
	public string StartedOnString
	{
		get
		{
			if (StartedOn.HasValue)
				return $"{StartedOn.Value.ToShortDateString()} {StartedOn.Value.ToShortTimeString()}";
			else
				return "not started";
		}
	}
	public DateTime? CompletedOn { get; set; } = null;
	public string CompletedOnString
	{
		get
		{
			if (CompletedOn.HasValue)
				return $"{CompletedOn.Value.ToShortDateString()} {CompletedOn.Value.ToShortTimeString()}";
			else
				return "not completed";
		}
	}
	public ITfDataProviderSychronizationLog SynchronizationLog { get; set; }
	
	public TucDataProviderSyncTask() { }
	
	public TucDataProviderSyncTask(TfDataProviderSynchronizeTask model)
	{
		Id = model.Id;
		Status = (TucSynchronizationStatus)(int)model.Status;
		CreatedOn = model.CreatedOn;
		StartedOn = model.StartedOn;
		CompletedOn = model.CompletedOn;
		SynchronizationLog = model.SynchronizationLog;
	}
}
