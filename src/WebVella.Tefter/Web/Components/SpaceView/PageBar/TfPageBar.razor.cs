namespace WebVella.Tefter.Web.Components;
public partial class TfPageBar : TfBaseComponent
{
	[Parameter]
	public List<MenuItem> Items { get; set; } = new();

	[Parameter] public string Style { get; set; }

	[Parameter] public RenderFragment Actions { get; set; }


}