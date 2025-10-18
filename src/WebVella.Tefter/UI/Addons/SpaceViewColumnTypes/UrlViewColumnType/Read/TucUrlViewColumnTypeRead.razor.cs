namespace WebVella.Tefter.UI.Addons;

public partial class TucUrlViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public TfUrlViewColumnTypeSettings Settings { get; set; } = null!;
}
