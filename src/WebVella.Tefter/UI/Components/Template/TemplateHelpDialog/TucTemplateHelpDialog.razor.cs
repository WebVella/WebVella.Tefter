
namespace WebVella.Tefter.UI.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateHelpDialog.TfTemplateHelpDialog", "WebVella.Tefter")]
public partial class TucTemplateHelpDialog : TfBaseComponent, IDialogContentComponent<TfTemplate?>
{
	[Parameter] public TfTemplate? Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

	private string _error = string.Empty;

	private ITfTemplateProcessorAddon? _processor = null;
	private TfTemplateProcessorHelpScreenRegionContext? _dynamicComponentContext = null;
	private TfScreenRegionScope? _dynamicComponentScope = null;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) _error = "Content is null";
		_initDynamicComponent();
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private ITfTemplateProcessorAddon? _getProcessor()
	{

		if (Content is not null && Content.ContentProcessorType is not null && Content.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessorAddon)) != null)
		{
			return (ITfTemplateProcessorAddon?)Activator.CreateInstance(Content.ContentProcessorType);

		}
		return null;

	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();

		_dynamicComponentContext = new TfTemplateProcessorHelpScreenRegionContext{};
		_dynamicComponentScope = new TfScreenRegionScope();
		if(_processor is not null)
			_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(), null);
	}


}

