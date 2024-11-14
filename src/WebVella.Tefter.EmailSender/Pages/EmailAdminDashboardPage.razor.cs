namespace WebVella.Tefter.EmailSender.Pages;

public partial class EmailAdminDashboardPage : TucBaseScreenRegionComponent, ITucAuxDataUseComponent, ITfScreenRegionComponent
{
	[Inject] protected IState<TfAppState> TfAppState { get; set; }
	[Inject] protected IState<TfAuxDataState> TfAuxDataState { get; set; }
	[Inject] protected NavigationManager Navigator { get; set; }
	[Inject] public IDispatcher Dispatcher { get; set; }
	public Guid Id { get { return new Guid("1f6e544e-6a53-4fa1-98ef-9c51a569c2b5"); } }
	public TfScreenRegion ScreenRegion { get { return TfScreenRegion.AdminPages; } }
	public int Position { get { return 1000; } }
	public string Name { get { return "Email Sender"; } }
	public string FluentIconName => "Mail";

	public Task OnAppStateInit(IServiceProvider serviceProvider, TucUser currentUser,
		TfAppState newAppState,
		TfAppState oldAppState, TfAuxDataState newAuxDataState, TfAuxDataState oldAuxDataState)
	{
		var mailService = serviceProvider.GetRequiredService<IEmailService>();
		var srvResult = mailService.GetEmailMessages(newAppState.Route.Search, newAppState.Route.Page, newAppState.Route.PageSize);
		if (srvResult.IsSuccess)
			newAuxDataState.Data[TfEmailSenderConstants.APP_EMAIL_LIST_DATA_KEY] = srvResult.Value;
		else
			newAuxDataState.Data[TfEmailSenderConstants.APP_EMAIL_LIST_DATA_KEY] = new List<EmailMessage>();

		return Task.CompletedTask;
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
			if (TfAppState.Value.Route.Page == 1)
			{
				var items = (List<EmailMessage>)TfAuxDataState.Value.Data[TfEmailSenderConstants.APP_EMAIL_LIST_DATA_KEY];
				items.Insert(0, (EmailMessage)result.Data);
				Dispatcher.Dispatch(new SetAuxDataStateByKeyAction(component: this,
					dictKey: TfEmailSenderConstants.APP_EMAIL_LIST_DATA_KEY,
					dictData: items));
			}
		}
	}

	private async Task _searchValueChanged(string search)
	{
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.SearchQueryName] = search;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
		await InvokeAsync(StateHasChanged);
	}

	private async Task _goFirstPage()
	{
		if (TfAppState.Value.Route.Page == 1) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = 1;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goPreviousPage()
	{
		var page = TfAppState.Value.Route.Page - 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goNextPage()
	{
		var page = TfAppState.Value.Route.Page + 1;
		if (page < 1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;

		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goLastPage()
	{
		if (TfAppState.Value.Route.Page == -1) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = -1;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}
	private async Task _goOnPage(int page)
	{
		if (page < 1 && page != -1) page = 1;
		if (TfAppState.Value.Route.Page == page) return;
		var queryDict = new Dictionary<string, object>();
		queryDict[TfConstants.PageQueryName] = page;
		await Navigator.ApplyChangeToUrlQuery(queryDict);
	}


}