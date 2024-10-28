namespace WebVella.Tefter.Web.Components;
public partial class TfTreeView : ComponentBase
{
	[Parameter]
	public List<TucMenuItem> Items { get; set; } = new();

	[Parameter]
	public string Style { get; set; }

	[Parameter]
	public string Class { get; set; }

	private string _cssClass { get => $"tf-menu {Class}"; }
}