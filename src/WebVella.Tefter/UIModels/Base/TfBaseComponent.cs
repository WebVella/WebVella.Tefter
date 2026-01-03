using Microsoft.AspNetCore.Components.Forms;
using ITfEventBus = WebVella.Tefter.UI.EventsBus.ITfEventBus;
namespace WebVella.Tefter.Models;

public class TfBaseComponent : ComponentBase
{
	[Inject] protected IServiceProvider ServiceProvider { get; set; } = null!;
	[Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
	[Inject] protected NavigationManager Navigator { get; set; } = null!;
	[Inject] protected IToastService ToastService { get; set; } = null!;
	[Inject] protected IDialogService DialogService { get; set; } = null!;
	[Inject] protected IMessageService MessageService { get; set; } = null!;
	[Inject] protected ITfConfigurationService ConfigurationService { get; set; } = null!;
	[Inject] protected IStringLocalizerFactory StringLocalizerFactory { get; set; } = null!;
	[Inject] protected IKeyCodeService KeyCodeService { get; set; } = null!;
	[Inject] protected ITfService TfService { get; set; } = null!;
	[Inject] protected ITfMetaService TfMetaService { get; set; } = null!;
	[CascadingParameter(Name = "TfAuthLayout")] public TfAuthLayout TfAuthLayout { get; set; } = null!;
	[Inject] protected ITfEventBus TfEventBus { get; set; } = null!;
	[Parameter] public Guid ComponentId { get; set; } = Guid.NewGuid();

	protected IStringLocalizer LC = null!;
	protected static IStringLocalizer? GL = null;
	protected string UriInitialized = String.Empty;
	private static AsyncLock _lock = new();


	public bool IsRenderLockEnabled { get; private set; } = false;
	public Guid CurrentRenderLock { get; private set; } = Guid.Empty;
	public Guid OldRenderLock { get; private set; } = Guid.Empty;

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
				var assemblyName = new AssemblyName(typeof(TfService).Assembly.FullName!);
				GL = StringLocalizerFactory.Create("Resources.SharedResource", assemblyName.Name!);
			}
		}
	}

	protected override void OnParametersSet()
	{
		if (IsRenderLockEnabled) RegenRenderLock();
	}

	protected override bool ShouldRender()
	{
		if (!IsRenderLockEnabled)
		{
			return true;
		}

		if (CurrentRenderLock == OldRenderLock)
		{
			return false;
		}

		OldRenderLock = CurrentRenderLock;
		return base.ShouldRender();
	}

	protected void EnableRenderLock()
	{
		IsRenderLockEnabled = true;
	}

	protected void DisableRenderLock()
	{
		IsRenderLockEnabled = false;
	}

	protected void RegenRenderLock() => CurrentRenderLock = Guid.NewGuid();

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
	//protected void ProcessServiceResponse(Result<object> response)
	//{
	//	ResultUtils.ProcessServiceResult(
	//		result: response,
	//		toastErrorMessage: LOC("Unexpected Error! Check Notifications for details"),
	//		toastValidationMessage: "Invalid Data",
	//		notificationErrorTitle: LOC("Unexpected Error!"),
	//		toastService: ToastService,
	//		messageService: MessageService
	//	);
	//}

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

	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	protected void ProcessFormSubmitException(Exception ex, EditContext editContext,
		ValidationMessageStore messageStore)
	{
		ResultUtils.ProcessFormSubmitException(
			exception: ex,
			toastErrorMessage: LOC("Unexpected Error! Check Notifications for details"),
			notificationErrorTitle: LOC("Unexpected Error!"),
			editContext: editContext,
			messageStore: messageStore,
			toastService: ToastService,
			messageService: MessageService
		);
	}
}
