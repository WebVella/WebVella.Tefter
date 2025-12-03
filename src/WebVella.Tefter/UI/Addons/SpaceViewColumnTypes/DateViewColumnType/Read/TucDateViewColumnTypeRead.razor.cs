namespace WebVella.Tefter.UI.Addons;

public partial class TucDateViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public List<DateOnly?>? Value { get; set; }
	
	private string _format = null!;

	protected override void OnParametersSet()
	{
		var settings = Context.GetSettings<TfDateViewColumnTypeSettings>();
		string defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
		_format = !String.IsNullOrWhiteSpace(settings.Format) ? settings.Format : defaultFormat;		
	}
}
