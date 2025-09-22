namespace WebVella.Tefter.UI.Components;

public partial class TucPageLayout : TfBaseComponent
{
	[Parameter] public RenderFragment? NavigationLeft { get; set; }
	[Parameter] public RenderFragment? NavigationRight { get; set; }
	[Parameter] public RenderFragment? ToolbarLeft { get; set; }
	[Parameter] public RenderFragment? ToolbarRight { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public bool ShowNavigation { get; set; } = true;
	[Parameter] public bool ShowToolbar { get; set; } = true;
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public string? Area { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public Icon? Icon { get; set; } = null;
	[Parameter] public TfColor? Color { get; set; } = null;
	private string? _returnUrl = null;
	private string _cssClass
	{
		get
		{
			List<string> classes = new List<string>();
			classes.Add("tf-page-layout");
			if (_showNavigation)
			{
				classes.Add($"tf-page-layout--with-navigation");
			}
			if (_showToolbar)
			{
				classes.Add($"tf-page-layout--with-toolbar");
			}
			return String.Join(" ", classes);
		}
	}

	private string _iconStyle
	{
		get
		{
			if (Color is null) return "";
			return $"background-color:var({Color.GetAttribute().Variable})";
		}
	}

	private bool _showNavigation
	{
		get
		{
			if (!String.IsNullOrWhiteSpace(Title) ||
				(ShowNavigation
				&& (NavigationLeft is not null || NavigationRight is not null)))
				return true;

			return false;
		}
	}

	private bool _showToolbar
	{
		get
		{
			if (ShowToolbar && (ToolbarLeft is not null || ToolbarRight is not null))
				return true;

			return false;
		}
	}
}