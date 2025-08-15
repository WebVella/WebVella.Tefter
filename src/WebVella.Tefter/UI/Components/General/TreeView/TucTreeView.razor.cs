namespace WebVella.Tefter.UI.Components;
public partial class TucTreeView : ComponentBase
{
	[Parameter]
	public List<TfMenuItem> Items { get; set; } = new();

	[Parameter]
	public string? Style { get; set; }

	[Parameter]
	public string? Class { get; set; }

	private string _cssClass { get => $"tf-treemenu {Class}"; }
}