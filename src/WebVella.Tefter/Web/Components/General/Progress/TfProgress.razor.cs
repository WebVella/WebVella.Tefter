namespace WebVella.Tefter.Web.Components;
public partial class TfProgress : ComponentBase
{
	[Parameter] public int? Value { get; set; }
	[Parameter] public bool Visible { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }

	private int _value { get=> Value is null ? 0 : Value.Value;}
	private string _cssClass { get => $"tf-progress {Class}";}

}