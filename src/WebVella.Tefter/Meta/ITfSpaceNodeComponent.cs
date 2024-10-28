namespace WebVella.Tefter;

public interface ITfSpaceNodeComponent
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Icon { get; set; }
	public TfSpaceNodeComponentContext Context { get; set; }
	public Task OnNodeCreated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public Task OnNodeUpdated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public Task OnNodeDeleted(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public Task<(TfAppState,TfAuxDataState)> InitState(
		IServiceProvider serviceProvider,
		TucUser currentUser,
		TfRouteState routeState,
		TfAppState newAppState, TfAppState oldAppState,
		TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState, 
		TfSpaceNodeComponentContext context);
	public string GetOptions();
	public List<ValidationError> ValidateOptions();
}

public class TfSpaceNodeComponentMeta
{
	public Type ComponentType { get; init; }
	internal ITfSpaceNodeComponent Instance { get; init; }
}

public class TfSpaceNodeComponentContext
{
	public Guid SpaceNodeId { get; set; }
	public Guid SpaceId { get; set; }
	public string Icon { get; set; }
	public string ComponentOptionsJson { get; set; }
	public TfComponentMode Mode { get; set; }
}