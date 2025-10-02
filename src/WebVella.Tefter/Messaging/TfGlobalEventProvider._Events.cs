namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Action<SampleGlobalEvent> SampleGlobalEvent;
	
	//space view column
	public event Action<TfSpaceViewColumnsChangedEvent> SpaceViewColumnsChangedEvent;
	
	//space view
	public event Action<TfSpaceViewUpdatedEvent> SpaceViewUpdatedEvent;
	
	//template
	public event Action<TfTemplateCreatedEvent> TemplateCreatedEvent;
	public event Action<TfTemplateUpdatedEvent> TemplateUpdatedEvent;
	public event Action<TfTemplateDeletedEvent> TemplateDeletedEvent;
	
	//user
	public event Action<TfUserCreatedEvent> UserCreatedGlobalEvent;
	public event Action<TfUserUpdatedEvent> UserUpdatedGlobalEvent;
	


	private void OnEventReceived(ITfEvent obj)
	{
		if (obj is SampleGlobalEvent)
			SampleGlobalEvent?.Invoke((SampleGlobalEvent)obj);
		
		//space view column
		if (obj is TfSpaceViewColumnsChangedEvent)
			SpaceViewColumnsChangedEvent?.Invoke((TfSpaceViewColumnsChangedEvent)obj);		
		
		//space view
		if (obj is TfSpaceViewUpdatedEvent)
			SpaceViewUpdatedEvent?.Invoke((TfSpaceViewUpdatedEvent)obj);

	
		//template
		if (obj is TfTemplateCreatedEvent)
			TemplateCreatedEvent?.Invoke((TfTemplateCreatedEvent)obj);		
		
		if (obj is TfTemplateUpdatedEvent)
			TemplateUpdatedEvent?.Invoke((TfTemplateUpdatedEvent)obj);
	
		if (obj is TfTemplateDeletedEvent)
			TemplateDeletedEvent?.Invoke((TfTemplateDeletedEvent)obj);				
		
		//user
		if (obj is TfUserCreatedEvent)
			UserCreatedGlobalEvent?.Invoke((TfUserCreatedEvent)obj);
		
		if (obj is TfUserUpdatedEvent)
			UserUpdatedGlobalEvent?.Invoke((TfUserUpdatedEvent)obj);					
	}
}
