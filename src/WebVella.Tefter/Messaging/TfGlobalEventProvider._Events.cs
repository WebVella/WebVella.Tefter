namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Func<SampleGlobalEvent,Task> SampleGlobalEvent;
	
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
