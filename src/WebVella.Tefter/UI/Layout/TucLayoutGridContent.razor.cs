namespace WebVella.Tefter.UI.Components;
public partial class TucLayoutGridContent : ComponentBase
{
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public int ColumnCount { get; set; } = 1;
	[Parameter] public string? ContentStyle { get; set; } = null;
	[Parameter] public string? BackUrl { get; set; } = null;
	[Parameter] public string? BackText { get; set; } = "Back";

	private string _layoutClass = "tf-layout-grid-content";

	private string _cssClass
	{
		get
		{
			List<string> classes = new List<string>();
			classes.Add("tf-layout");
			classes.Add(_layoutClass);
			return String.Join(" ", classes);
		}
	}

	private string _contentStyle
	{
		get
		{
			List<string> styles = new List<string>();
			if (ColumnCount > 1)
			{
				List<string> frString = new();
				for (int i = 0; i < ColumnCount; i++)
				{
					frString.Add("1fr");
				}
				styles.Add($"grid-template-columns: {String.Join(" ", frString)};");
			}

			if (!String.IsNullOrWhiteSpace(ContentStyle))
				styles.Add(ContentStyle);

			if(styles.Count == 0)
				return String.Empty;
			return String.Join(";",styles);
		}

	}
}