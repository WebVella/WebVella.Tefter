namespace WebVella.Tefter.UI.Addons;

public partial class TucDateViewColumnTypeRead : ComponentBase
{
	[Parameter] public List<DateOnly?>? Value { get; set; }
	[Parameter] public TfDateViewColumnTypeSettings Settings { get; set; } = null!;
	
	private string _format = null!;

	protected override void OnInitialized()
	{
		string defaultFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
		_format = !String.IsNullOrWhiteSpace(Settings.Format) ? Settings.Format : defaultFormat;
	}
}
