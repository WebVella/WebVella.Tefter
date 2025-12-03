namespace WebVella.Tefter.UI.Addons;

public partial class TucDateTimeViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadMode Context { get; set; } = null!;
	[Parameter] public List<DateTime?>? Value { get; set; }
	
	private string _format = null!;

	protected override void OnParametersSet()
	{
		var settings = Context.GetSettings<TfDateTimeViewColumnTypeSettings>();
		string defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
		_format = !String.IsNullOrWhiteSpace(settings.Format) ? settings.Format : defaultFormat;		
	}	
}
