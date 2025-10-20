namespace WebVella.Tefter.UI.Addons;

public partial class TucTimeViewColumnTypeRead : ComponentBase
{
	[Parameter] public TfSpaceViewColumnReadModeContext Context { get; set; } = null!;	
	[Parameter] public List<DateTime?>? Value { get; set; }
	
	private string _format = null!;

	protected override void OnInitialized()
	{
		var settings = Context.GetSettings<TfTimeViewColumnTypeSettings>();
		string defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;
		_format = !String.IsNullOrWhiteSpace(settings.Format) ? settings.Format : defaultFormat;
	}
}
