namespace WebVella.Tefter.Web.Store.UserDetailsState;

public record GetUserDetailsAction
{
	public Guid RecordId { get; }

	public GetUserDetailsAction(Guid recordId)
	{
		RecordId = recordId;
	}
}
