namespace WebVella.Tefter.Messaging;


public partial class TfGlobalEventProvider : IAsyncDisposable
{
	public event Action<SampleGlobalEvent> SampleGlobalEvent;
	
	//data identity
	public event Action<TfDataIdentityCreatedEvent> DataIdentityCreatedEvent;
	public event Action<TfDataIdentityUpdatedEvent> DataIdentityUpdatedEvent;
	public event Action<TfDataIdentityDeletedEvent> DataIdentityDeletedEvent;		
	
	//data provider
	public event Action<TfDataProviderCreatedEvent> DataProviderCreatedEvent;
	public event Action<TfDataProviderUpdatedEvent> DataProviderUpdatedEvent;
	public event Action<TfDataProviderDeletedEvent> DataProviderDeletedEvent;			
	
	//dataset
	public event Action<TfDatasetCreatedEvent> DatasetCreatedEvent;
	public event Action<TfDatasetUpdatedEvent> DatasetUpdatedEvent;
	public event Action<TfDatasetDeletedEvent> DatasetDeletedEvent;		
	
	//repository file
	public event Action<TfRepositoryFileCreatedEvent> RepositoryFileCreatedEvent;
	public event Action<TfRepositoryFileUpdatedEvent> RepositoryFileUpdatedEvent;
	public event Action<TfRepositoryFileDeletedEvent> RepositoryFileDeletedEvent;		
	
	//role
	public event Action<TfRoleCreatedEvent> RoleCreatedEvent;
	public event Action<TfRoleUpdatedEvent> RoleUpdatedEvent;
	public event Action<TfRoleDeletedEvent> RoleDeletedEvent;		
	
	//shared column
	public event Action<TfSharedColumnCreatedEvent> SharedColumnCreatedEvent;
	public event Action<TfSharedColumnUpdatedEvent> SharedColumnUpdatedEvent;
	public event Action<TfSharedColumnDeletedEvent> SharedColumnDeletedEvent;			
	
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
			UserCreatedGlobalEvent?.Invoke((TfUserCreatedEvent)obj);
		
		if (obj is TfUserUpdatedEvent)
			UserUpdatedGlobalEvent?.Invoke((TfUserUpdatedEvent)obj);					
	}
}
