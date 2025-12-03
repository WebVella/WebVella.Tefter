namespace WebVella.Tefter.UI.Addons;

public partial class TucImageViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public List<string?>? Value { get; set; }
}
