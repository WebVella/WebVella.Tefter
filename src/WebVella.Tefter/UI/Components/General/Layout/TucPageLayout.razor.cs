namespace WebVella.Tefter.UI.Components;
public partial class TucPageLayout : ComponentBase
{
	[Parameter] public RenderFragment? Toolbar { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public bool ShowToolbar { get; set; } = false;
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public Icon? Icon { get; set; } = null;
	[Parameter] public TfColor? Color { get; set; } = null;

	private string _cssClass
	{
		get
		{
			List<string> classes = new List<string>();
			classes.Add("tf-page-layout");
			if (_showToolbar)
			{
				classes.Add($"tf-page-layout--with-toolbar");
			}
			return String.Join(" ", classes);
		}
	}

	private bool _showToolbar
	{
		get
		{
			if (!String.IsNullOrWhiteSpace(Title) || ShowToolbar)
				return true;

			return false;
		}
	}
}