namespace WebVella.Tefter.UseCases.Models;

public record TucDataProviderSyncTaskInfoLog
{
	public Guid TaskId { get; set; }

	public TucDataProviderSyncTaskInfoType Type { get; set; } = TucDataProviderSyncTaskInfoType.Info;

}
