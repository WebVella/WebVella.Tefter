
namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Web.Components.Admin.TemplateSettingsDialog.TfTemplateSettingsDialog", "WebVella.Tefter")]
public partial class TfTemplateSettingsDialog : TfBaseComponent, IDialogContentComponent<TucTemplate>
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] private AppStateUseCase UC { get; set; }
	[Parameter] public TucTemplate Content { get; set; }

	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isSubmitting = false;
	private string _title = "";
	private string _btnText = "";
	private Icon _iconBtn;
	private ITfTemplateProcessor _processor = null;
	private string _form = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if (Content is null) throw new Exception("Content is null");
		_title = LOC("Update settings");
		_btnText = LOC("Save");
		_iconBtn = TfConstants.GetIcon("Save");
		_processor = _getProcessor();
		_form = Content.SettingsJson;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{

			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _save()
	{
		if (_isSubmitting) return;
		try
		{
			//Workaround to wait for the form to be bound 
			//on enter click without blur
			await Task.Delay(10);
			////Check form

			_isSubmitting = true;
			await InvokeAsync(StateHasChanged);
			var submitResult = UC.UpdateTemplateSettings(Content.Id,_form);
			ProcessServiceResponse(submitResult);
			if (submitResult.IsSuccess)
			{
				await Dialog.CloseAsync(submitResult.Value);
			}
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_isSubmitting = false;
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private Dictionary<string, object> _getDynamicComponentParams()
	{
		var dict = new Dictionary<string, object>();
		dict["DisplayMode"] = TfComponentMode.Update;
		dict["Value"] = _form;
		dict["ValueChanged"] = EventCallback.Factory.Create<string>(this, _settingsChanged);
		return dict;
	}

	private void _settingsChanged(string json)
	{
		_form = json;
	}

	private ITfTemplateProcessor _getProcessor()
	{
		var context = Content;
		if (context is null) return null;
		if (context.ContentProcessorType is not null && context.ContentProcessorType.GetInterface(nameof(ITfTemplateProcessor)) != null)
		{
			return (ITfTemplateProcessor)Activator.CreateInstance(context.ContentProcessorType);

		}
		return null;

	}

}

