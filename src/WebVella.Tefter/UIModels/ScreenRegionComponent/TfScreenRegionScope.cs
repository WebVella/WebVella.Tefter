namespace WebVella.Tefter.Models;

public record TfScreenRegionScope
{
	/// <summary>
	/// matched towards the type of the presented item. Null - for no restriction
	/// </summary>
	public Type? ItemType { get;  private set; } = null;

	/// <summary>
	/// matched towards the Id of the region component. Null - for no restriction
	/// </summary>
	public Guid? ComponentId { get; private set; } = null;

	public TfScreenRegionScope(){}

	public TfScreenRegionScope(Type itemType, Guid? componentId){
		ItemType = itemType;
		ComponentId = componentId;
	}
}
