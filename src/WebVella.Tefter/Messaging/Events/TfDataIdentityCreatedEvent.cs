namespace WebVella.Tefter.Messaging;

public class TfDataIdentityCreatedEvent : TfGlobalEvent
{
	public TfDataIdentity Payload { get; set; } = null!;

	public TfDataIdentityCreatedEvent() { }

	public TfDataIdentityCreatedEvent(TfDataIdentity payload)
	{
		Payload = payload;
	}
}