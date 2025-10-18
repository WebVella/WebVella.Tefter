namespace WebVella.Tefter.UI.Addons;

public partial class TucTimeViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<DateTime?>? Value { get; set; }
	[Parameter] public TfTimeViewColumnTypeSettings Settings { get; set; } = null!;
	
	private string _format = null!;

	protected override void OnInitialized()
	{
		string defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern;
		_format = !String.IsNullOrWhiteSpace(Settings.Format) ? Settings.Format : defaultFormat;
	}
}
