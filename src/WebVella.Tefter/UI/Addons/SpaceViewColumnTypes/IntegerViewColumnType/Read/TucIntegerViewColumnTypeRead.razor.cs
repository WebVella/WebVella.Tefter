namespace WebVella.Tefter.UI.Addons;

public partial class TucIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<int?>? Value { get; set; }
	[Parameter] public TfIntegerViewColumnTypeSettings Settings { get; set; } = null!;
}
