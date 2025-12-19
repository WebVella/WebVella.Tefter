namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Func<SampleGlobalEvent,Task> SampleGlobalEvent;
	
	
	//space
	public event Func<TfSpaceCreatedEvent,Task> SpaceCreatedEvent;
	public event Func<TfSpaceUpdatedEvent,Task> SpaceUpdatedEvent;
	public event Func<TfSpaceDeletedEvent,Task> SpaceDeletedEvent;		
	
	//space page
	public event Func<TfSpacePageCreatedEvent,Task> SpacePageCreatedEvent;
	public event Func<TfSpacePageUpdatedEvent,Task> SpacePageUpdatedEvent;
	public event Func<TfSpacePageDeletedEvent,Task> SpacePageDeletedEvent;	
	
	//space view column
	public event Func<TfSpaceViewColumnsChangedEvent,Task> SpaceViewColumnsChangedEvent;
	
	//space view data
	public event Func<TfSpaceViewDataChangedEvent,Task> SpaceViewDataChangedEvent;	
	
	//space view
	public event Func<TfSpaceViewUpdatedEvent,Task> SpaceViewUpdatedEvent;
	
	//template
	public event Func<TfTemplateCreatedEvent,Task> TemplateCreatedEvent;
	public event Func<TfTemplateUpdatedEvent,Task> TemplateUpdatedEvent;
	public event Func<TfTemplateDeletedEvent,Task> TemplateDeletedEvent;
	
	//user
	public event Func<TfUserCreatedEvent,Task> UserCreatedEvent;
	public event Func<TfUserUpdatedEvent,Task> UserUpdatedEvent;
	


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
		
		//space view data
		if (obj is TfSpaceViewDataChangedEvent)
			SpaceViewDataChangedEvent?.Invoke((TfSpaceViewDataChangedEvent)obj);				
		
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
			UserCreatedEvent?.Invoke((TfUserCreatedEvent)obj);
		
		if (obj is TfUserUpdatedEvent)
			UserUpdatedEvent?.Invoke((TfUserUpdatedEvent)obj);					
					
	}
}
