using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Nito.AsyncEx;

namespace WebVella.Tefter.Web.Components.BaseComponent;

public class TfBaseComponent : FluxorComponent
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
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
	protected static IStringLocalizer GL = null;
	private static AsyncLock _lock = new();
	protected override void OnInitialized()
	{
		base.OnInitialized();
		LC = StringLocalizerFactory.Create(this.GetType());
		if (GL is null)
		{
			using (_lock.Lock())
			{
				GL = StringLocalizerFactory.Create(this.GetType().BaseType);
			}
		}
	}

	protected string LOC(string key, params object[] arguments)
	{
		if (LC[key, arguments] != key) return LC[key, arguments];
		if (GL[key, arguments] != key) return GL[key, arguments];
		return key;
	}

	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	protected void ProcessResponse(Result response)
	{
		if (response.IsSuccess) return;

		ToastService.ShowToast(ToastIntent.Error, "ex.Message");
	}


	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	protected void ProcessException(Exception ex)
	{
		ToastService.ShowToast(ToastIntent.Error, ex.Message);
	}

}
