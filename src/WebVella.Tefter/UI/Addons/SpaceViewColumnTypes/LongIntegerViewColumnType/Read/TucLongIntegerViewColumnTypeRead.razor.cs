namespace WebVella.Tefter.UI.Addons;

public partial class TucLongIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<long?>? Value { get; set; }

	private TfLongIntegerViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfLongIntegerViewColumnTypeSettings>();
	}	
}
