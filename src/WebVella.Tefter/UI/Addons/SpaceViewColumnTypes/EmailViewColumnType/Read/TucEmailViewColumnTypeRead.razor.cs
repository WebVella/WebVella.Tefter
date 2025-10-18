namespace WebVella.Tefter.UI.Addons;

public partial class TucEmailViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public TfEmailViewColumnTypeSettings Settings { get; set; } = null!;
}
