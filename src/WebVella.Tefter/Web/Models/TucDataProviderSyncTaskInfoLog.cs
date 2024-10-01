namespace WebVella.Tefter.Web.Models;

public record TucDataProviderSyncTaskInfoLog
{
	public Guid TaskId { get; set; }

	public TucDataProviderSyncTaskInfoType Type { get; set; } = TucDataProviderSyncTaskInfoType.Info;

}
