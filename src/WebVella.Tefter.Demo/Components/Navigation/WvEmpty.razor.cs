namespace WebVella.Tefter.Demo.Components;
public partial class WvEmpty : WvBaseComponent
{
	[Parameter]
	public string Text { get; set; } = "No items found";
}