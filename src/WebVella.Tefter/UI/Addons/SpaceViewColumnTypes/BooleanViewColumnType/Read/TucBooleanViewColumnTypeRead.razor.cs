namespace WebVella.Tefter.UI.Addons;

public partial class TucBooleanViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<bool?>? Value { get; set; }
	[Parameter] public TfBooleanViewColumnTypeSettings Settings { get; set; } = null!;
}
