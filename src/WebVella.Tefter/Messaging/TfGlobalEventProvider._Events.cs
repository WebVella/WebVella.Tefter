namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Func<SampleGlobalEvent,Task> SampleGlobalEvent;
	
	//data identity
	public event Func<TfDataIdentityCreatedEvent,Task> DataIdentityCreatedEvent;
	public event Func<TfDataIdentityUpdatedEvent,Task> DataIdentityUpdatedEvent;
	public event Func<TfDataIdentityDeletedEvent,Task> DataIdentityDeletedEvent;		
	
	//data provider
	public event Func<TfDataProviderCreatedEvent,Task> DataProviderCreatedEvent;
	public event Func<TfDataProviderUpdatedEvent,Task> DataProviderUpdatedEvent;
	public event Func<TfDataProviderDeletedEvent,Task> DataProviderDeletedEvent;			
	
	//dataset
	public event Func<TfDatasetCreatedEvent,Task> DatasetCreatedEvent;
	public event Func<TfDatasetUpdatedEvent,Task> DatasetUpdatedEvent;
	public event Func<TfDatasetDeletedEvent,Task> DatasetDeletedEvent;		
	
	//repository file
	public event Func<TfRepositoryFileCreatedEvent,Task> RepositoryFileCreatedEvent;
	public event Func<TfRepositoryFileUpdatedEvent,Task> RepositoryFileUpdatedEvent;
	public event Func<TfRepositoryFileDeletedEvent,Task> RepositoryFileDeletedEvent;		
	
	//role
	public event Func<TfRoleCreatedEvent,Task> RoleCreatedEvent;
	public event Func<TfRoleUpdatedEvent,Task> RoleUpdatedEvent;
	public event Func<TfRoleDeletedEvent,Task> RoleDeletedEvent;		
	
	//shared column
	public event Func<TfSharedColumnCreatedEvent,Task> SharedColumnCreatedEvent;
	public event Func<TfSharedColumnUpdatedEvent,Task> SharedColumnUpdatedEvent;
	public event Func<TfSharedColumnDeletedEvent,Task> SharedColumnDeletedEvent;			
	
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
		
		//data identity
		if (obj is TfDataIdentityCreatedEvent)
			DataIdentityCreatedEvent?.Invoke((TfDataIdentityCreatedEvent)obj);		
		
		if (obj is TfDataIdentityUpdatedEvent)
			DataIdentityUpdatedEvent?.Invoke((TfDataIdentityUpdatedEvent)obj);
	
		if (obj is TfDataIdentityDeletedEvent)
			DataIdentityDeletedEvent?.Invoke((TfDataIdentityDeletedEvent)obj);				
		
		//data provider
		if (obj is TfDataProviderCreatedEvent)
			DataProviderCreatedEvent?.Invoke((TfDataProviderCreatedEvent)obj);		
		
		if (obj is TfDataProviderUpdatedEvent)
			DataProviderUpdatedEvent?.Invoke((TfDataProviderUpdatedEvent)obj);
	
		if (obj is TfDataProviderDeletedEvent)
			DataProviderDeletedEvent?.Invoke((TfDataProviderDeletedEvent)obj);			
		
		
		//dataset
		if (obj is TfDatasetCreatedEvent)
			DatasetCreatedEvent?.Invoke((TfDatasetCreatedEvent)obj);		
		
		if (obj is TfDatasetUpdatedEvent)
			DatasetUpdatedEvent?.Invoke((TfDatasetUpdatedEvent)obj);
	
		if (obj is TfDatasetDeletedEvent)
			DatasetDeletedEvent?.Invoke((TfDatasetDeletedEvent)obj);				
		
		//repository file
		if (obj is TfRepositoryFileCreatedEvent)
			RepositoryFileCreatedEvent?.Invoke((TfRepositoryFileCreatedEvent)obj);		
		
		if (obj is TfRepositoryFileUpdatedEvent)
			RepositoryFileUpdatedEvent?.Invoke((TfRepositoryFileUpdatedEvent)obj);
	
		if (obj is TfRepositoryFileDeletedEvent)
			RepositoryFileDeletedEvent?.Invoke((TfRepositoryFileDeletedEvent)obj);				
		
		//role
		if (obj is TfRoleCreatedEvent)
			RoleCreatedEvent?.Invoke((TfRoleCreatedEvent)obj);		
		
		if (obj is TfRoleUpdatedEvent)
			RoleUpdatedEvent?.Invoke((TfRoleUpdatedEvent)obj);
	
		if (obj is TfRoleDeletedEvent)
			RoleDeletedEvent?.Invoke((TfRoleDeletedEvent)obj);			
		
		
		//role
		if (obj is TfRoleCreatedEvent)
			RoleCreatedEvent?.Invoke((TfRoleCreatedEvent)obj);		
		
		if (obj is TfRoleUpdatedEvent)
			RoleUpdatedEvent?.Invoke((TfRoleUpdatedEvent)obj);
	
		if (obj is TfRoleDeletedEvent)
			RoleDeletedEvent?.Invoke((TfRoleDeletedEvent)obj);			
		
		//shared column
		if (obj is TfSharedColumnCreatedEvent)
			SharedColumnCreatedEvent?.Invoke((TfSharedColumnCreatedEvent)obj);		
		
		if (obj is TfSharedColumnUpdatedEvent)
			SharedColumnUpdatedEvent?.Invoke((TfSharedColumnUpdatedEvent)obj);
	
		if (obj is TfSharedColumnDeletedEvent)
			SharedColumnDeletedEvent?.Invoke((TfSharedColumnDeletedEvent)obj);			
		
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
			UserCreatedEvent?.Invoke((TfUserCreatedEvent)obj);
		
		if (obj is TfUserUpdatedEvent)
			UserUpdatedEvent?.Invoke((TfUserUpdatedEvent)obj);					
	}
}
