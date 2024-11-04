namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutHeader : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Aside { get; set; }

	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__header");
			if (Aside is not null) sb.Append(" tf-layout__header--has-aside");
			return String.Join(" ", sb);
		}
	}
}