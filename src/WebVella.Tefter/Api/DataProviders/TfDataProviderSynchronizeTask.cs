namespace WebVella.Tefter;

public class TfDataProviderSynchronizeTask
{
	public TfDataProvider DataProvider { get; set; }
	public TfSynchronizationPolicy Policy { get; set; }
	public TfSynchronizationStatus Status { get; set; } = TfSynchronizationStatus.Pending;
	public DateTime CreatedOn { get; set; } = DateTime.Now;
	public DateTime? StartedOn { get; set; } = null;
	public DateTime? CompletedOn { get; set; } = null;
	public TfSynchronizationResult Result { get; set; }
}
