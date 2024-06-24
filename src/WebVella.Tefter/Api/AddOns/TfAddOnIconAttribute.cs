namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfAddOnIconAttribute : Attribute
{
	public string IconResourceName { get; private set; }
	public Stream Icon => this.GetType().Assembly.GetManifestResourceStream(IconResourceName);

	public TfAddOnIconAttribute(string iconResourceName )
	{
		IconResourceName = iconResourceName;
	}
}
