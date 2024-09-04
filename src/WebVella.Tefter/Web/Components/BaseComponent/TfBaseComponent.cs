namespace WebVella.Tefter.Web.Components;

public class TfBaseComponent : FluxorComponent
{
	[Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	[Inject] public IActionSubscriber ActionSubscriber { get; set; }
	[Inject] protected IJSRuntime JSRuntime { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] protected IToastService ToastService { get; set; }
	[Inject] protected IDialogService DialogService { get; set; }
	[Inject] protected IMessageService MessageService { get; set; }
	[Inject] protected IWebConfigurationService ConfigurationService { get; set; }
	[Inject] protected ITfService TfSrv { get; set; }
	[Inject] protected IIdentityManager IdentityManager { get; set; }
	[Inject] protected ITfDataProviderManager DataProviderManager { get; set; }
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; }
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

	private bool hasLogging = false;
	protected IStringLocalizer LC;
	protected static IStringLocalizer GL = null;
	private static AsyncLock _lock = new();
	protected override void OnInitialized()
	{
		base.OnInitialized();
		var type = this.GetType();
		var (resourceBaseName, resourceLocation) = type.GetLocalizationResourceInfo();
		if (!String.IsNullOrWhiteSpace(resourceBaseName) && !String.IsNullOrWhiteSpace(resourceLocation))
		{
			LC = StringLocalizerFactory.Create(resourceBaseName, resourceLocation);
		}
		else
		{
			LC = StringLocalizerFactory.Create(type);
		}
		if (GL is null)
		{
			using (_lock.Lock())
			{
				GL = StringLocalizerFactory.Create(type.BaseType);
			}
		}

#if DEBUG
		if (hasLogging) Console.WriteLine($"+++++++ Initialized {type.Name}");
#endif
	}

	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);
#if DEBUG
		if (hasLogging) Console.WriteLine($"+++++++ Render {this.GetType().Name}");
#endif
	}

	protected string LOC(string key, params object[] arguments)
	{
		if (LC is not null && LC[key, arguments] != key) return LC[key, arguments];
		if (GL is not null && GL[key, arguments] != key) return GL[key, arguments];
		return key;
	}

	/// <summary>
	/// Processes Exception from Server call.
	/// Important: Just non validation errors will be treated as errors.
	/// Validation messages will not be processed by this method
	/// </summary>
	/// <param name="ex"></param>
	protected void ProcessServiceResponse(Result<object> response)
	{
		ResultUtils.ProcessServiceResult(
			result: response,
			toastErrorMessage: LOC("Unexpected Error! Check Notifications for details"),
			notificationErrorTitle: LOC("Unexpected Error!"),
			toastService: ToastService,
			messageService: MessageService
		);
	}


	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	protected string ProcessException(Exception ex)
	{
		return ResultUtils.ProcessException(
			exception: ex,
			toastErrorMessage: LOC("Unexpected Error! Check Notifications for details"),
			notificationErrorTitle: LOC("Unexpected Error!"),
			toastService: ToastService,
			messageService: MessageService
		);
	}


}
