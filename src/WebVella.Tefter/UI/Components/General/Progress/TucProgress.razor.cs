namespace WebVella.Tefter.UI.Components;
public partial class TucProgress : ComponentBase
{
	[Parameter] public int? Value { get; set; }
	[Parameter] public bool Visible { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }
	[Parameter] public bool LabelVisible { get; set; } = true;

	private int _value { get=> Value is null ? 0 : Value.Value;}
	private string _cssClass { get => $"tf-progress {Class}";}

}