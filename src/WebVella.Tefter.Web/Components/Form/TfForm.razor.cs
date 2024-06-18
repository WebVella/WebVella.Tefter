namespace WebVella.Tefter.Web.Components;
public partial class TfForm : TfBaseComponent
{

	[Parameter]
	public RenderFragment ChildContent { get; set; }

	[Parameter]
	public string Style { get; set; }
}