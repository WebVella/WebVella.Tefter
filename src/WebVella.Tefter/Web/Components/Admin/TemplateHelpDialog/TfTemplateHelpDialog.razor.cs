
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateHelpDialog.TfTemplateHelpDialog", "WebVella.Tefter")]
public partial class TfTemplateHelpDialog : TfBaseComponent, IDialogContentComponent<ITfTemplateProcessor>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public ITfTemplateProcessor Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) _error = "Content is null";
		if(Content.HelpComponentType is null) _error = LOC("Processor does not have help");
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Read;
		dict["Context"] = new TfTemplateProcessorHelpComponentContext(){};
		return dict;
	}
}

