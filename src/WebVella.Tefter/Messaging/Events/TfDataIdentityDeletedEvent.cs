namespace WebVella.Tefter.Messaging;

public class TfDataIdentityDeletedEvent : TfGlobalEvent
{
	public TfDataIdentity Payload { get; set; } = null!;

	public TfDataIdentityDeletedEvent() { }

	public TfDataIdentityDeletedEvent(TfDataIdentity payload)
	{
		Payload = payload;
	}
}