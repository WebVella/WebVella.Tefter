namespace WebVella.Tefter.Messaging;

public class SampleGlobalEvent : IGlobalEvent
{
	public Guid Id { get; init; }

	public string Message { get; set; }
}
