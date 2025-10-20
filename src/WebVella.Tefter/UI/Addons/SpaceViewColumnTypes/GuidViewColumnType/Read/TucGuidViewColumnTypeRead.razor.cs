namespace WebVella.Tefter.UI.Addons;

public partial class TucGuidViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<Guid?>? Value { get; set; }
}
