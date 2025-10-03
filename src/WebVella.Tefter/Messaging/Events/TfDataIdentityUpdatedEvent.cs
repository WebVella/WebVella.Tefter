namespace WebVella.Tefter.Messaging;

public class TfDataIdentityUpdatedEvent : TfGlobalEvent
{
	public TfDataIdentity Payload { get; set; } = null!;

	public TfDataIdentityUpdatedEvent() { }

	public TfDataIdentityUpdatedEvent(TfDataIdentity payload)
	{
		Payload = payload;
	}
}