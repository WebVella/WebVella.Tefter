namespace WebVella.Tefter.UI.Addons;

public partial class TucUrlViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<string?>? Value { get; set; }
}
