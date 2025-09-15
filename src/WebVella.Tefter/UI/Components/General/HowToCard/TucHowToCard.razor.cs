namespace WebVella.Tefter.UI.Components;
public partial class TucHowToCard : TfBaseComponent
{
	[Parameter] public RenderFragment? ChildContent { get; set; }
	[Parameter] public string? Title { get; set; }
	[Parameter] public string? SubTitle { get; set; }
	[Parameter] public string? Style { get; set; }
	[Parameter] public List<TfHowToItem> Items { get; set; } = new();
}