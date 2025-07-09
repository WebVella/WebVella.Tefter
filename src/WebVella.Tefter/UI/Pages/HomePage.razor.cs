namespace WebVella.Tefter.UI.Pages;
public partial class HomePage : TfBasePage
{
	[Inject] protected IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		WvBlazorTraceService.OnSignal(caller: this, signalName: "HomePage OnInitialized");
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		WvBlazorTraceService.OnSignal(caller: this, signalName: "HomePage OnAfterRender");
	}
}