namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfApplicationTypeAttribute : Attribute
{
	private Guid _id;
	private string _name;
	private string _description;
	private string _iconResourceName;
	private bool _allowMultipleInstances;

	public Guid Id => _id;
	public string Name => _name;
	public string Description => _description;
	public Stream Icon => Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(_iconResourceName);

	public TfApplicationTypeAttribute(string id, string name, string description,  
		string iconResourceName = null, bool allowMultipleInstances = true)
	{
		_id = new Guid(id);
		_name = name;
		_description = description;
		_iconResourceName = iconResourceName;
		_allowMultipleInstances = allowMultipleInstances;
	}
}
