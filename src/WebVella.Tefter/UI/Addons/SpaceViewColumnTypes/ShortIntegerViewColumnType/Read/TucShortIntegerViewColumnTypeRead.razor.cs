namespace WebVella.Tefter.UI.Addons;

public partial class TucShortIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<short?>? Value { get; set; }
	
	private TfShortIntegerViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfShortIntegerViewColumnTypeSettings>();
	}	
}
