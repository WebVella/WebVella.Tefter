namespace WebVella.Tefter.Messaging;

public class SampleGlobalEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public Guid? UserId { get; set; } = null;

	public string Message { get; set; }
}
