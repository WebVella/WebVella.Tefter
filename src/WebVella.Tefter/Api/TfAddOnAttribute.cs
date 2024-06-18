namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfAddOnAttribute : Attribute
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }

	public TfAddOnAttribute(string Id, string Name, string Description)
	{
		this.Id = new Guid(Id);
		this.Name = Name;
		this.Description = Description;
	}
}
