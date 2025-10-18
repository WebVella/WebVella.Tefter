namespace WebVella.Tefter.UI.Addons;

public partial class TucLongIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<long?>? Value { get; set; }
	[Parameter] public TfLongIntegerViewColumnTypeSettings Settings { get; set; } = null!;
}
