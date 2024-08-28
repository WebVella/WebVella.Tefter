namespace WebVella.Tefter;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
public class TucEnumMatchAttribute : Attribute
{
	public Type EnumType { get; private set; }
	public TucEnumMatchAttribute(Type enumType)
	{
		this.EnumType = enumType;
	}
}
