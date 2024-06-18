namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class TfColumnIconAttribute : Attribute
{
	public string IconResourceName { get; private set; }
	public Stream Icon => this.GetType().Assembly.GetManifestResourceStream(IconResourceName);

	public TfColumnIconAttribute(string iconResourceName )
	{
		IconResourceName = iconResourceName;
	}
}
