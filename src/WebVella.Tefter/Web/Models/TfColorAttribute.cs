namespace WebVella.Tefter.Web.Models;
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class TfColorAttribute : Attribute
{
	public string Name { get; private set; }
	public string Value { get; private set; }
	public string Variable { get; private set; }
	public int Number { get; private set; }
	public bool Selectable { get; private set; }
	public TfColorAttribute(string name, string value, string variable,int number, bool selectable)
	{
		this.Name = name;
		this.Value = value;
		this.Variable = variable;
		this.Number = number;
		this.Selectable = selectable;
	}
}

