namespace WebVella.Tefter.UI.Addons;

public partial class TucPhoneViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<string?>? Value { get; set; }
	[Parameter] public TfPhoneViewColumnTypeSettings Settings { get; set; } = null!;
}
