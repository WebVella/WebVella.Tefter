namespace WebVella.Tefter.UI.Addons;

public partial class TucNumberViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<decimal?>? Value { get; set; }
	[Parameter] public TfNumberViewColumnTypeSettings Settings { get; set; } = null!;
}
