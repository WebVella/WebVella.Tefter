namespace WebVella.Tefter.Web.Components;
public partial class TfLayout : ComponentBase
{
	[Parameter] public bool SidebarExpanded { get; set; }
	[Parameter] public RenderFragment Header { get; set; }
	[Parameter] public RenderFragment Navigation { get; set; }
	[Parameter] public RenderFragment Body { get; set; }
	[Parameter] public TfColor Color { get; set; } = TfColor.Emerald500;
	private string _styles
	{
		get
		{
			var colorName = Color.GetAttribute().Name;
			var sb = new StringBuilder();
			sb.AppendLine("<style>");
			sb.AppendLine("html:root .tf-layout__body, html:root .tf-layout__header {");
			sb.AppendLine($"--accent-base-color: var(--tf-{colorName}-500);");
			sb.AppendLine($"--accent-fill-rest: var(--tf-{colorName}-500);");
			sb.AppendLine($"--accent-fill-hover: var(--tf-{colorName}-600);");
			sb.AppendLine($"--accent-fill-active: var(--tf-{colorName}-700);");
			sb.AppendLine($"--accent-fill-focus: var(--tf-{colorName}-600);");


			sb.AppendLine($"--accent-foreground-rest: var(--tf-{colorName}-700);");
			sb.AppendLine($"--accent-foreground-hover: var(--tf-{colorName}-500);");
			sb.AppendLine($"--accent-foreground-active: var(--tf-{colorName}-400);");
			sb.AppendLine($"--accent-foreground-focus: var(--tf-{colorName}-600);");
			sb.AppendLine($"--accent-stroke-control-rest: linear-gradient(var(--tf-{colorName}-600) 90%, var(--tf-{colorName}-700) 100%);");
			sb.AppendLine($"--accent-stroke-control-hover: linear-gradient(var(--tf-{colorName}-500) 90%, var(--tf-{colorName}-600) 100%);");
			sb.AppendLine($"--accent-stroke-control-active: var(--tf-{colorName}-500);");
			sb.AppendLine($"--accent-stroke-control-focus: linear-gradient(var(--tf-{colorName}-400) 90%, var(--tf-{colorName}-700) 100%);");


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