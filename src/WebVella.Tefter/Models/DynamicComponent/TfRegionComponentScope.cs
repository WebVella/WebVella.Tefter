﻿namespace WebVella.Tefter.Models;

public record TfRegionComponentScope
{
	/// <summary>
	/// matched towards the type of the presented item. Null - for no restriction
	/// </summary>
	public Type ItemType { get;  private set; } = null;

	/// <summary>
	/// matched towards the ID of the region component. Null - for no restriction
	/// </summary>
	public Guid? ComponentId { get; private set; } = null;

	public TfRegionComponentScope(){}

	public TfRegionComponentScope(Type itemType, Guid? componentId){
		ItemType = itemType;
		ComponentId = componentId;
	}
}
