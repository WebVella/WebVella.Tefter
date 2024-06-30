namespace WebVella.Tefter.Web.Store.UserAdminState;

public record GetUserAdminAction
{
	public Guid RecordId { get; }

	public GetUserAdminAction(Guid recordId)
	{
		RecordId = recordId;
	}
}
