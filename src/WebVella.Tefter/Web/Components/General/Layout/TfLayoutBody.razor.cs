namespace WebVella.Tefter.Web.Components;
public partial class TfLayoutBody
{
	[Parameter] public RenderFragment Aside { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }
	[Parameter] public string Color { get; set; }
	private string _styles
	{
		get
		{
			var sb = new StringBuilder();
			sb.AppendLine("<style>");
			sb.AppendLine(":root .tf-layout__body {");
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
			sb.Append("tf-layout__body");
			if (ChildContent is not null) sb.Append(" tf-layout__body--has-aside");
			return String.Join(" ", sb);
		}
	}

}