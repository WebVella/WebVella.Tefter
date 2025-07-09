namespace WebVella.Tefter.UI.Components;
public partial class TucLayoutListDetails : ComponentBase
{
	[Parameter] public RenderFragment? Header { get; set; }
	[Parameter] public RenderFragment? Toolbar { get; set; }
	[Parameter] public RenderFragment? AsideToolbar { get; set; }
	[Parameter] public RenderFragment? AsideContent { get; set; }
	[Parameter] public RenderFragment? ContentToolbar { get; set; }
	[Parameter] public RenderFragment? Content { get; set; }
	private string _layoutClass = "tf-layout-list-details";
	private string _cssClass
	{
		get
		{
			List<string> classes = new List<string>();
			classes.Add("tf-layout");
			classes.Add(_layoutClass);
			if (Toolbar is not null)
			{
				classes.Add($"{_layoutClass}--with-toolbar");
			}
			return String.Join(" ",classes);
		}
	}
}