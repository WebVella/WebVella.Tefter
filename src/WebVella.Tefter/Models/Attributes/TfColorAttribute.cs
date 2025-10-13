namespace WebVella.Tefter.Models;
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class TfColorAttribute : Attribute
{
	public string Name { get; private set; }
	public string OKLCH { get; private set; }
	
	public string HEX { get; private set; }
	public string Variable { get; private set; }
	public int Number { get; private set; }
	public bool Selectable { get; private set; }

	public bool UseWhiteForeground
	{
		get
		{
			if(Number >= 500) return true;
			return false;
		}
	}

	public TfColorAttribute(string name, string oklch,string variable, int number, bool selectable, string hex = "#fff")
	{
		this.Name = name;
		this.OKLCH = oklch;
		this.HEX = hex;
		this.Variable = variable;
		this.Number = number;
		this.Selectable = selectable;
	}
}

