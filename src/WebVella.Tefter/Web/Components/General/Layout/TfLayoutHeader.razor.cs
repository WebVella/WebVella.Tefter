namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutHeader
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Aside { get; set; }

}