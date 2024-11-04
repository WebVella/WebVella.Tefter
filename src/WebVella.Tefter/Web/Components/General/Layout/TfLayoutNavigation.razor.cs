namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutNavigation : ComponentBase
{
	[Parameter] public RenderFragment Top { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Bottom { get; set; }

	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__navigation");
			if (Top is not null) sb.Append(" tf-layout__navigation--has-top");
			if (Bottom is not null) sb.Append(" tf-layout__navigation--has-bottom");
			return String.Join(" ", sb);
		}
	}
}