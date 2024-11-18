namespace WebVella.Tefter.Web.Components;
public partial class TfPageHeader : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public string Class { get; set; }
	[Parameter] public string Style { get; set; }
	[Parameter] public bool IsSubHeader { get; set; }

	private string _cssClass { get => $"page-header {Class} {(IsSubHeader? "page-subheader" : "" )}";}

}