namespace WebVella.Tefter.Web.Store.DataProviderAdminState;

public record GetDataProviderAdminAction
{
	public Guid RecordId { get; }

	public GetDataProviderAdminAction(Guid recordId)
	{
		RecordId = recordId;
	}
}
