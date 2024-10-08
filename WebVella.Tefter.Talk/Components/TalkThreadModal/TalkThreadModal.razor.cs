namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkThreadModal.TalkThreadModal", "WebVella.Tefter.Talk")]
public partial class TalkThreadModal : TfFormBaseComponent, IDialogContentComponent<TalkChannel>
{
	[Inject] public IState<TfUserState> TfUserState { get; set; }
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkChannel Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _title = string.Empty;
	private string _error = string.Empty;
	private bool _isLoading = false;
	private bool _isSubmitting = false;
	private TfEditor _mainEditor = null;
	private TfEditor _subEditor = null;
	private Guid? _activeThreadId = null;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		await Task.Delay(100);
		await _mainEditor.Focus();
	}

	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _save()
	{
		ToastService.ShowInfo("send");
		_activeThreadId = _activeThreadId is null ? Guid.Empty : null;
		
	}


}
