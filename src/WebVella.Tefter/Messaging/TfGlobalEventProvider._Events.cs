namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Action<SampleGlobalEvent> SampleGlobalEvent;
	
	//space
	public event Action<TfSpaceCreatedEvent> SpaceCreatedEvent;
	public event Action<TfSpaceUpdatedEvent> SpaceUpdatedEvent;
	public event Action<TfSpaceDeletedEvent> SpaceDeletedEvent;		
	
	//space page
	public event Action<TfSpacePageCreatedEvent> SpacePageCreatedEvent;
	public event Action<TfSpacePageUpdatedEvent> SpacePageUpdatedEvent;
	public event Action<TfSpacePageDeletedEvent> SpacePageDeletedEvent;	
	
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
		
		//space
		if (obj is TfSpaceCreatedEvent)
			SpaceCreatedEvent?.Invoke((TfSpaceCreatedEvent)obj);		
		
		if (obj is TfSpaceUpdatedEvent)
			SpaceUpdatedEvent?.Invoke((TfSpaceUpdatedEvent)obj);
	
		if (obj is TfSpaceDeletedEvent)
			SpaceDeletedEvent?.Invoke((TfSpaceDeletedEvent)obj);				
		
		//space page
		if (obj is TfSpacePageCreatedEvent)
			SpacePageCreatedEvent?.Invoke((TfSpacePageCreatedEvent)obj);		
		
		if (obj is TfSpacePageUpdatedEvent)
			SpacePageUpdatedEvent?.Invoke((TfSpacePageUpdatedEvent)obj);
	
		if (obj is TfSpacePageDeletedEvent)
			SpacePageDeletedEvent?.Invoke((TfSpacePageDeletedEvent)obj);			
		
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
