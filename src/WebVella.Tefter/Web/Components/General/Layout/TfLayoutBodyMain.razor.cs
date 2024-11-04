namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBodyMain : ComponentBase
{
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public RenderFragment Header { get; set; }
	[Parameter] public RenderFragment HeaderAside { get; set; }
	[Parameter] public RenderFragment Toolbar { get; set; }
	[Parameter] public RenderFragment ToolbarAside { get; set; }
	[Parameter] public RenderFragment Footer { get; set; }
	[Parameter] public string ContentClass { get; set; }

	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__main");
			if (Header is not null || HeaderAside is not null) sb.Append(" tf-layout__body__main--has-header");
			if (Toolbar is not null || ToolbarAside is not null) sb.Append(" tf-layout__body__main--has-toolbar");
			if (Footer is not null) sb.Append(" tf-layout__body__main--has-footer");
			return String.Join(" ", sb);
		}
	}

	private string _contentClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout__body__main__content");
			if (!String.IsNullOrWhiteSpace(ContentClass)) sb.Append($" {ContentClass}");
			return String.Join(" ", sb);
		}
	}
	

}