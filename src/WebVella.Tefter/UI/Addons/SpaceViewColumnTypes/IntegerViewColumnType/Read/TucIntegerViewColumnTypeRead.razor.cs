namespace WebVella.Tefter.UI.Addons;

public partial class TucIntegerViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;
	[Parameter] public List<int?>? Value { get; set; }
	
	private TfIntegerViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfIntegerViewColumnTypeSettings>();
	}	

}
