namespace WebVella.Tefter.Demo.Components;

public class WvBaseComponent : ComponentBase
{
	[CascadingParameter(Name = "WvState")]
	protected WvState WvState { get; set; }

	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

}
