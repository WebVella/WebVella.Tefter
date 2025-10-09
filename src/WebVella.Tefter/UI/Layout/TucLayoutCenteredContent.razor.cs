namespace WebVella.Tefter.UI.Components;
public partial class TucLayoutCenteredContent : ComponentBase
{
	[Parameter] public RenderFragment? Content { get; set; }

	private string _layoutClass = "tf-layout-centered-content";

	private string _cssClass
	{
		get
		{
			List<string> classes = new List<string>();
			classes.Add("tf-layout");
			classes.Add(_layoutClass);
			return String.Join(" ",classes);
		}
	}
}