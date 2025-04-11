
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateHelpDialog.TfTemplateHelpDialog", "WebVella.Tefter")]
public partial class TfTemplateHelpDialog : TfBaseComponent, IDialogContentComponent<TucTemplate>
{
	[Parameter] public TucTemplate Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;

	private ITfTemplateProcessor _processor = null;
	private TfTemplateProcessorHelpScreenRegionContext _dynamicComponentContext = null;
	private TfScreenRegionScope _dynamicComponentScope = null;

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

	private ITfTemplateProcessor _getProcessor()
	{

		if (Content.ContentProcessorType is not null && Content.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			return (ITfTemplateProcessor)Activator.CreateInstance(Content.ContentProcessorType);

		}
		return null;

	}

	private void _initDynamicComponent()
	{
		_processor = _getProcessor();

		_dynamicComponentContext = new TfTemplateProcessorHelpScreenRegionContext
		{
		};
		_dynamicComponentScope = new TfScreenRegionScope(_processor.GetType(),null);
	}


}

