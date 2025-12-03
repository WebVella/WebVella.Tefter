namespace WebVella.Tefter.UI.Components;

public partial class TucTileCardActions : TfBaseComponent
{
	[Parameter] public List<TfMenuItem> Menu { get; set; } = new();
	private bool _open = false;
	private readonly string _itemId = "comp-" + Guid.NewGuid();
}