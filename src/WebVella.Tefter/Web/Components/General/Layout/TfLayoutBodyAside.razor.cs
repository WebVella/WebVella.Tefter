namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBodyAside : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Toolbar { get; set; }

	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__aside");
			if (Toolbar is not null) sb.Append(" tf-layout__body__aside--has-toolbar");
			return String.Join(" ", sb);
		}
	}

}