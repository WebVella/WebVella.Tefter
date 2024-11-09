namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBodyMainToolbar : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Aside { get; set; }
	[Parameter] public string Class { get; set; }
	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__main__toolbar");
			if (Aside is not null) sb.Append(" tf-layout__body__main__toolbar--has-aside");
			if (!String.IsNullOrWhiteSpace(Class))
				sb.Append($" {Class}");
			return String.Join(" ", sb);
		}
	}

}