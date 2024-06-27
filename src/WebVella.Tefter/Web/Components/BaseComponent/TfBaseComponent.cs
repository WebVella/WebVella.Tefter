using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Nito.AsyncEx;
using WebVella.Tefter.Errors;

namespace WebVella.Tefter.Web.Components.BaseComponent;

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
	[Inject] protected IConfigurationService ConfigurationService { get; set; }
	[Inject] protected ITfService TfSrv { get; set; }
	[Inject] protected IIdentityManager IdentityManager { get; set; }
	[Inject] protected ITfDataProviderManager DataProviderManager { get; set; }
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
	protected void ProcessServiceResponse(Result response)
	{
		if (response.IsSuccess) return;
		var generalErrors = new List<string>();
		foreach (IError iError in response.Errors)
		{
			if (iError is ValidationError)
			{
				var error = (ValidationError)iError;
				if (String.IsNullOrWhiteSpace(error.PropertyName))
					generalErrors.Add(error.Reason);
			}
			else
			{
				var error = (IError)iError;
				generalErrors.Add(error.Message);
			}

		}
		if (generalErrors.Count > 0)
		{
			ToastService.ShowToast(ToastIntent.Error, LOC("Unexpected Error! Check Notifications for details"));
			SendErrorsToNotifications(LOC("Unexpected Error!"), generalErrors);
		}
	}


	/// <summary>
	/// Processes Exception from Server call
	/// </summary>
	/// <param name="ex"></param>
	protected string ProcessException(Exception ex)
	{
		string errorMessage = LOC("Unexpected Error! Check Notifications for details");
		ToastService.ShowToast(ToastIntent.Error, errorMessage);
		SendErrorsToNotifications(LOC("Unexpected Error!"), new List<string> { ex.Message });

		return errorMessage;
	}
	/// <summary>
	/// Send error details to notification center
	/// </summary>
	/// <param name="message"></param>
	/// <param name="errors"></param>
	protected void SendErrorsToNotifications(string message, List<string> errors)
	{
		var divHtml = "<ul class='notification-list'>";
		foreach (var error in errors)
		{
			divHtml += $"<li>{error}</li>";
		}
		divHtml += "</ul>";

		MessageService.ShowMessageBar(options =>
		{
			options.Intent = MessageIntent.Error;
			options.Title = message;
			options.Body = divHtml;
			options.Timestamp = DateTime.Now;
			options.Timeout = 15000;
			options.AllowDismiss = true;
			options.Section = TfConstants.MESSAGES_NOTIFICATION_CENTER;
		});
	}

}
