using Fluxor.Blazor.Web.Components;

namespace WebVella.Tefter.Demo.Components;

public class WvBaseComponent : FluxorComponent, IAsyncDisposable
{
	[CascadingParameter(Name = "WvState")]
	protected WvState WvState { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Inject] protected IWvService WvService { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

}
