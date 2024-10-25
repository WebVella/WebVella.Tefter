﻿namespace WebVella.Tefter;

public interface ITfSpaceNodeComponent
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public TfSpaceNodeComponentContext Context { get; set; }
	public Task OnNodeCreated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public Task OnNodeUpdated(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public Task OnNodeDeleted(IServiceProvider serviceProvider, TfSpaceNodeComponentContext context);
	public string GetOptions();
	public List<ValidationError> ValidateOptions();
}

public class TfSpaceNodeComponentMeta
{
	public Guid Id { get { return Instance.Id; } }
	public string Name { get { return Instance.Name; } }
	public string Description { get { return Instance.Description; } }
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