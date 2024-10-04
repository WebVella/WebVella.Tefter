namespace WebVella.Tefter.Web.Components;
[LocalizationResource("WebVella.Tefter.Talk.Components.TalkThreadPanel.TalkThreadPanel", "WebVella.Tefter.Talk")]
public partial class TalkThreadPanel : TfFormBaseComponent, IDialogContentComponent<TalkChannel>
{
	[Inject] public IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] public ITalkService TalkService { get; set; }
	[Parameter] public TalkChannel Content { get; set; }
	[CascadingParameter] public FluentDialog Dialog { get; set; }

	private string _error = string.Empty;
	private bool _isLoading = false;
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
	}
	private async Task _cancel()
	{
		await Dialog.CancelAsync();
	}

	private async Task _sendMessage(){ 
		ToastService.ShowInfo("send");
	}

}
