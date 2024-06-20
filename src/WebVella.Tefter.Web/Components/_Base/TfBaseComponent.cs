using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace WebVella.Tefter.Web.Components;

public class TfBaseComponent : FluxorComponent
{
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Inject] protected IConfigurationService ConfigurationService { get; set; }
	[Inject] protected ITfService TfSrv { get; set; }
	[Inject] protected ProtectedLocalStorage ProtectedLocalStorage { get; set; }
	[Inject] protected IIdentityManager IdentityManager { get; set; }
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

	protected IStringLocalizer LC;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		LC = StringLocalizerFactory.Create(this.GetType());
	}

	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	internal void ProcessException(Exception ex)
	{
		ToastService.ShowToast(ToastIntent.Error, ex.Message);
	}

}
