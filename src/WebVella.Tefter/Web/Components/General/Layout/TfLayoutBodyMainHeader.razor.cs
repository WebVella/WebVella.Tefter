namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBodyMainHeader
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Aside { get; set; }
	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__main__header");
			if (Aside is not null) sb.Append(" tf-layout__body__main__header--has-aside");
			return String.Join(" ", sb);
		}
	}

}