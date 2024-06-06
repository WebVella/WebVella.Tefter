namespace WebVella.Tefter.Demo.Components;

public class WvBasePage : ComponentBase
{
	[CascadingParameter(Name = "WvState")]
	protected WvState WvState { get; set; }

	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IWvService WvService { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

}
