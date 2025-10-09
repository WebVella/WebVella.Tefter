using Microsoft.AspNetCore.Components.Routing;

namespace WebVella.Tefter.EmailSender.Components;

public partial class EmailSenderLogAdmin : TfBaseComponent, IDisposable
{
    [Inject] public IEmailService EmailService { get; set; }

    private string? _search = null;
    private List<EmailMessage> _messages = new();
    private Guid? _submitingEmail = null;
    private TfNavigationState _navState = new();
    private FluentSearch? _refSearch = null;
    private TfUser? _currentUser = null;

    public void Dispose()
    {
        EmailService.EmailCreated -= On_EmailChanged;
        EmailService.EmailUpdated -= On_EmailChanged;
        Navigator.LocationChanged -= On_NavigationStateChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = TfAuthLayout.GetState().User;
        await _init(TfAuthLayout.GetState().NavigationState);
        EmailService.EmailCreated += On_EmailChanged;
        EmailService.EmailUpdated += On_EmailChanged;
        Navigator.LocationChanged += On_NavigationStateChanged;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && _refSearch != null)
        {
            _refSearch.FocusAsync();
        }
    }

    private async Task On_EmailChanged(EmailMessage args)
    {
        await InvokeAsync(async () => { await _init(TfAuthLayout.GetState().NavigationState); });
    }

    private void On_NavigationStateChanged(object? caller, LocationChangedEventArgs args)
    {
        InvokeAsync(async () =>
        {
            if (UriInitialized != args.Location)
                await _init(TfAuthLayout.GetState().NavigationState);
        });
    }

    private async Task _init(TfNavigationState navState)
    {
        _navState = navState;
        try
        {
            _messages = EmailService.GetEmailMessages(_navState.Search, _navState.Page ?? 1,
                _navState.PageSize ?? TfConstants.PageSize);
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
        var queryDict = new Dictionary<string, object>
        {
            { TfConstants.SearchQueryName, search }
        };
        await Navigator.ApplyChangeToUrlQuery(queryDict);
        await InvokeAsync(StateHasChanged);
    }

    private async Task _goLastPage()
    {
        if (_navState.Page == -1) return;
        var queryDict = new Dictionary<string, object>
        {
            { TfConstants.PageQueryName, -1 }
        };
        await Navigator.ApplyChangeToUrlQuery(queryDict);
    }

    private async Task _goOnPage(int page)
    {
        if (page < 1 && page != -1) page = 1;
        if (_navState.Page == page) return;
        var queryDict = new Dictionary<string, object>
        {
            { TfConstants.PageQueryName, page }
        };
        await Navigator.ApplyChangeToUrlQuery(queryDict);
    }
}