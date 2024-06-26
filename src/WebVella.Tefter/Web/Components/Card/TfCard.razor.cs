namespace WebVella.Tefter.Web.Components.Card;
public partial class TfCard : TfBaseComponent
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	[Parameter]
	public string Title { get; set; }
}