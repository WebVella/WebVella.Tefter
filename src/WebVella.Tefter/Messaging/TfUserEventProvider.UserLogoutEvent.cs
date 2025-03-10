namespace WebVella.Tefter.Messaging;

public class UserLogoutEvent : IUserEvent
{
	public Guid Id { get; init; }

	public TfUser User { get; set; }
}
