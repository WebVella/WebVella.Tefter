using Microsoft.JSInterop;
using WebVella.Tefter.UIServices;

namespace WebVella.Tefter.EmailSender.Components;
[LocalizationResource("WebVella.Tefter.EmailSender.Components.EmailSenderLogAdmin.EmailSenderLogAdmin", "WebVella.Tefter.EmailSender")]
public partial class EmailSenderLogAdmin : TfBaseComponent, IDisposable
{
	[Inject] public IEmailService EmailService { get; set; }
	[Inject] public ITfNavigationUIService TfNavigationUIService { get; set; } = default!;

	private string? _search = null;
	private List<EmailMessage> _messages = new();
	private Guid? _submitingEmail = null;
	private TfNavigationState _navState = new();
	public void Dispose()
	{
		EmailService.EmailCreated -= On_EmailChanged;
		EmailService.EmailUpdated -= On_EmailChanged;
		TfNavigationUIService.NavigationStateChanged -= On_NavigationStateChanged;
	}
	protected override async Task OnInitializedAsync()
	{
		await _init();
		EmailService.EmailCreated += On_EmailChanged;
		EmailService.EmailUpdated += On_EmailChanged;
		TfNavigationUIService.NavigationStateChanged += On_NavigationStateChanged;
	}
	private async void On_EmailChanged(object? caller, EmailMessage args)
	{
		await _init();
	}
	private async void On_NavigationStateChanged(object? caller, TfNavigationState args)
	{
		if (UriInitialized != args.Uri)
			await _init(args);
	}

	private async Task _init(TfNavigationState? navState = null)
	{
		if (navState == null)
			_navState = await TfNavigationUIService.GetNavigationStateAsync(Navigator);
		else
			_navState = navState;
		try
		{
			_messages = EmailService.GetEmailMessages(_navState.Search, _navState.Page ?? 1, _navState.PageSize ?? TfConstants.PageSize);
			_search = _navState.Search;
		}
		finally
		{
			UriInitialized = _navState.Uri;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _viewEmailHandler(EmailMessage message)
	{
		var dialog = await DialogService.ShowDialogAsync<ViewEmailDialog>(
		message,
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{


		}
	}

	private async Task _resentEmailHandler(EmailMessage message)
	{
		_submitingEmail = message.Id;
		await InvokeAsync(StateHasChanged);
		try
		{

			var result = EmailService.ResendEmailMessage(message.Id);
			ToastService.ShowSuccess(LOC("Message scheduled for sending"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitingEmail = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _cancelEmailHandler(EmailMessage message)
	{
		_submitingEmail = message.Id;
		await InvokeAsync(StateHasChanged);
		try
		{

			var result = EmailService.CancelEmailMessage(message.Id);
			ToastService.ShowSuccess(LOC("Message sending canceled"));
		}
		catch (Exception ex)
		{
			ProcessException(ex);
		}
		finally
		{
			_submitingEmail = null;
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _createTestHandler()
	{
		var dialog = await DialogService.ShowDialogAsync<SendTestEmailDialog>(
		new EmailMessage(),
		new DialogParameters()
		{
			PreventDismissOnOverlayClick = true,
			PreventScroll = true,
			Width = TfConstants.DialogWidthLarge,
			TrapFocus = false
		});
		var result = await dialog.Result;
		if (!result.Cancelled && result.Data != null)
		{
		}
	}

	private async Task _searchValueChanged(string search)
	{
		var queryDict = new Dictionary<string, object>{
			{TfConstants.SearchQueryName,search}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		await InvokeAsync(StateHasChanged);
	}

	private async Task _goFirstPage()
	{
		if (_navState.Page == 1) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName,1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = _navState.Page - 1;
		if (page < 1) page = 1;
		if (_navState.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		var page = _navState.Page + 1;
		if (page < 1) page = 1;
		if (_navState.Page == page) return;

		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName,page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (_navState.Page == -1) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, -1}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (_navState.Page == page) return;
		var queryDict = new Dictionary<string, object>{
			{TfConstants.PageQueryName, page}
		};
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
}