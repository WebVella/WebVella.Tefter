namespace WebVella.Tefter.Web.Components.PageBar;
public partial class TfPageBar : TfBaseComponent
{
	[Parameter]
	public List<MenuItem> Items { get; set; } = new();

	[Parameter] public string Style { get; set; }

	[Parameter] public RenderFragment Actions { get; set; }


}