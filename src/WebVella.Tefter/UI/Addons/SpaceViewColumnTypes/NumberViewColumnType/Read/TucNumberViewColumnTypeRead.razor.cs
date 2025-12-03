namespace WebVella.Tefter.UI.Addons;

public partial class TucNumberViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public List<decimal?>? Value { get; set; }

	private TfNumberViewColumnTypeSettings _settings =  new ();

	protected override void OnParametersSet()
	{
		_settings = Context.GetSettings<TfNumberViewColumnTypeSettings>();
	}
}
