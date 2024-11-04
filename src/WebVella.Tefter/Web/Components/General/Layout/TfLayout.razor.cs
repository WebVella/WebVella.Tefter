namespace WebVella.Tefter.Web.Components;
public partial class TfLayout
{
	[Parameter] public bool SidebarExpanded { get; set; }
	[Parameter] public RenderFragment Header { get; set; }
	[Parameter] public RenderFragment Navigation { get; set; }
	[Parameter] public RenderFragment Body { get; set; }
	[Parameter] public string Color { get; set; }
	[Parameter] public string BackgroundColor { get; set; }

	private string _styles
	{
		get
		{
			var sb = new StringBuilder();
			sb.AppendLine("<style>");
			sb.AppendLine(":root .tf-layout {");
			if (!String.IsNullOrWhiteSpace(Color))
			{
				sb.AppendLine($"--accent-base-color: {Color};");
				sb.AppendLine($"--accent-fill-rest: {Color};");
			}
			sb.AppendLine("}");
			sb.AppendLine("</style>");
			return sb.ToString();
		}
	}
	private string _layoutClass
	{
		get
		{
			StringBuilder sb = new();
			sb.Append("tf-layout");
			if (!SidebarExpanded) sb.Append(" tf-layout--aside-collapsed");
			return sb.ToString();
		}
	}

}