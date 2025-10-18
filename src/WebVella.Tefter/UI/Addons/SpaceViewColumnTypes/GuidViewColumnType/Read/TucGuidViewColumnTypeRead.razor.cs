namespace WebVella.Tefter.UI.Addons;

public partial class TucGuidViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<Guid?>? Value { get; set; }
	[Parameter] public TfGuidViewColumnTypeSettings Settings { get; set; } = null!;
}
