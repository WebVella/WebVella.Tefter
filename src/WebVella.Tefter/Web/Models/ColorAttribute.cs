namespace WebVella.Tefter.Web.Models;
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class ColorAttribute : Attribute
{
	public string OKLCH { get; private set; }
	public ColorAttribute(string value)
	{
		this.OKLCH = value;
	}
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class SelectableAttribute : Attribute
{
	public string Label { get; private set; }
	public SelectableAttribute(string label)
	{
		this.Label = label;
	}
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class NameAttribute : Attribute
{
	public string Name { get; private set; }
	public NameAttribute(string name)
	{
		this.Name = name;
	}
}