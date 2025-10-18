namespace WebVella.Tefter.UI.Addons;

public partial class TucTextViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public TfTextViewColumnTypeSettings Settings { get; set; } = null!;
}
