namespace WebVella.Tefter.UI.Addons;

public partial class TucBooleanViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public List<bool?>? Value { get; set; }
	
	private TfBooleanViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfBooleanViewColumnTypeSettings>();
	}	
}
