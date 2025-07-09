namespace WebVella.Tefter.Messaging;

public class UserStateChangedEvent : IUserEvent
{
	public Guid Id { get; init; }
	public Guid StateComponentId { get; init; }
	public Guid ComponentId { get; init; }
	//public TfUserState State { get; set; }
}
