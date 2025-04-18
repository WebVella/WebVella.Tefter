namespace WebVella.Tefter.Web.Components;
public partial class TfPageHeader : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment SubHeader { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }
	private string _cssClass { get => $"page-header {Class}";}
}