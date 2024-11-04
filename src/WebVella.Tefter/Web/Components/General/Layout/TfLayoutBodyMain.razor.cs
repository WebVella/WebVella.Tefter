namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBodyMain
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Header { get; set; }
	[Parameter] public RenderFragment Toolbar { get; set; }
	[Parameter] public RenderFragment Footer { get; set; }

	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__main");
			if (Header is not null) sb.Append(" tf-layout__body__main--has-header");
			if (Toolbar is not null) sb.Append(" tf-layout__body__main--has-toolbar");
			if (Footer is not null) sb.Append(" tf-layout__body__main--has-footer");
			return String.Join(" ", sb);
		}
	}

}