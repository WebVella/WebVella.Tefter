namespace WebVella.Tefter.UI.Addons;

public partial class TucShortIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<short?>? Value { get; set; }
	[Parameter] public TfShortIntegerViewColumnTypeSettings Settings { get; set; } = null!;
}
