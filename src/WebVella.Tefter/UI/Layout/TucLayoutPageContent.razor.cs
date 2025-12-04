namespace WebVella.Tefter.UI.Components;

public partial class TucLayoutPageContent : TfBaseComponent
{
	[Parameter] public string? Title { get; set; } = null;
	[Parameter] public string? Icon { get; set; } = null;
	[Parameter] public RenderFragment? Tabs { get; set; }
	[Parameter] public RenderFragment? Toolbar { get; set; }
	[Parameter] public RenderFragment? Aside { get; set; }
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public EventCallback OnEdit { get; set; }
	[Parameter] public string? OnEditTooltop { get; set; } = null;
	[Parameter] public EventCallback OnAdd { get; set; }
	[Parameter] public string? OnAddTooltop { get; set; } = null;

	[Parameter] public bool ShowTabs { get; set; } = true;

	private string _cssClass
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append("tflpc ");
			if (Tabs is not null && ShowTabs)
			{
				sb.Append("tflpc--has-page-tabs ");
			}
			return sb.ToString();	
		}
	}
}