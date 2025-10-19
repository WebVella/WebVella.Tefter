namespace WebVella.Tefter.UI.Addons;

public partial class TucImageViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public TfImageViewColumnTypeSettings Settings { get; set; } = null!;
}
